using System;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration.CookControllerPowerTubeIntegration
{
    [TestFixture]
    public class CookControllerPowerTubeTest
    {


        private UserInterface _userInterface;
        private IDoor _door;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDisplay _display;
        private ILight _light;
        private CookController _topCookController;
        private IOutput _fakeOutput;
        private ITimer _timer;
        private IPowerTube _powerTubeToIntegrate;
        [SetUp]
        public void IntegrationTestSetup()
        {
            _fakeOutput = Substitute.For<IOutput>();

            _timer = new Timer();
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_fakeOutput);
            _display = new Display(_fakeOutput);
            _powerTubeToIntegrate = new PowerTube(_fakeOutput);
            _topCookController = new CookController(
                _timer,
                _display,
                _powerTubeToIntegrate);
            _userInterface = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _topCookController);
            _topCookController.UI = _userInterface;
        }

        [TestCase(1)]
        [TestCase(2)]
        public void CookController_CookControllerStarted_PowerTubeReceivedCorrectValue(int powerButtonPressed)
        {
            for (int i = 0; i < powerButtonPressed; i++)
            {
                _powerButton.Press();
            }
            _timeButton.Press();

            _startCancelButton.Press();

            _fakeOutput.Received().OutputLine(Arg.Is<string>(str => str.Contains($"{powerButtonPressed * 50}")));
        }

        [TestCase(5)]
        [TestCase(10)]
        public void CookController_CookControllerStartedWithToHighPower_PowerTubeThrewException(int powerButtonPressed)
        {
            try
            {
                for (int i = 0; i < powerButtonPressed; i++)
                {
                    _powerButton.Press();
                }
                _timeButton.Press();

                _startCancelButton.Press();

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.That(e,Is.TypeOf<ArgumentOutOfRangeException>());
            }
        }

        [Test]
        public void CookController_CookControllerStartedThenStartedAgain_PowerTubeThrewException()
        {
            try
            {
                _powerButton.Press();
                _timeButton.Press();
                _startCancelButton.Press();

                _topCookController.StartCooking(50,50);

                Assert.Fail();
            }
            catch (Exception e)
            {
                Assert.That(e, Is.TypeOf<ApplicationException>());
            }
        }

        [Test]
        public void CookController_DoorOpened_CookControllerStopped()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _door.Open();

            _fakeOutput.Received().OutputLine(Arg.Is<string>(str=>str.Contains("PowerTube turned off")));
        }


        [Test]
        public void CookController_TimerExpired_CookControllerStopped()
        {
            bool timerExpired = false;
            _timer.Expired += (sender, args) => timerExpired = true;
            _powerButton.Press();
            _timeButton.Press();

            _startCancelButton.Press();

            while(timerExpired != true) { }

            _fakeOutput.Received().OutputLine(Arg.Is<string>(str => str.Contains("PowerTube turned off")));
        }


        [Test]
        public void CookController_CancelPressed_CookControllerStopped()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();
            
            _fakeOutput.Received().OutputLine(Arg.Is<string>(str=>str.Contains("PowerTube turned off")));
        }
    }
}