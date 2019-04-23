using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration.UICookControllerIntegration
{
    [TestFixture]
    public class UICookControllerTest
    {
        private UserInterface _topUserInterface;
        private IDoor _door;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDisplay _display;
        private ILight _light;
        private CookController _cookControllerToIntegrate;
        private IOutput _fakeOutput;
        private ITimer _fakeTimer;
        private IPowerTube _fakePowerTube;
        [SetUp]
        public void IntegrationTestSetup()
        {
            _fakeOutput = Substitute.For<IOutput>();
            _fakeTimer = Substitute.For<ITimer>();
            _fakePowerTube = Substitute.For<IPowerTube>();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_fakeOutput);
            _display = new Display(_fakeOutput);
            _cookControllerToIntegrate = new CookController(
                _fakeTimer,
                _display,
                _fakePowerTube);
            _topUserInterface = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cookControllerToIntegrate);
            _cookControllerToIntegrate.UI = _topUserInterface;
        }

        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        public void UI_CookControllerStarted_TimerReceivedCorrectTime(int timeButtonPressed)
        {
            _powerButton.Press();
            for (int i = 0; i < timeButtonPressed; i++)
            {
                _timeButton.Press();
            }

            _startCancelButton.Press();

            _fakeTimer.Received().Start(Arg.Is(60*timeButtonPressed));
        }

        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        public void UI_CookControllerStarted_PowerTubeReceivedCorrectValue(int powerButtonPressed)
        {
            for (int i = 0; i < powerButtonPressed; i++)
            {
                _powerButton.Press();
            }
            _timeButton.Press();

            _startCancelButton.Press();

            _fakePowerTube.Received().TurnOn(Arg.Is(powerButtonPressed*50));
        }

        [Test]
        public void UI_DoorOpened_CookControllerStopped()
        {
            _powerButton.Press();   
            _timeButton.Press();
            _startCancelButton.Press();

            _door.Open();

            _fakePowerTube.Received().TurnOff();
        }


        [Test]
        public void UI_CancelPressed_CookControllerStopped()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _startCancelButton.Press();

            _fakePowerTube.Received().TurnOff();
        }
    }
}