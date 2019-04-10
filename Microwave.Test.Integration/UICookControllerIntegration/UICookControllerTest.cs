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
        private IUserInterface _topUserInterface;
        private IDoor _door;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDisplay _display;
        private ILight _light;
        private ICookController _cookControllerToIntegrate;
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
                _fakePowerTube,
                _topUserInterface);
            _topUserInterface = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cookControllerToIntegrate);
        }

        [Test]
        public void UI_CookControllerStarted_TimerReceivedCorrectTime()
        {
            _powerButton.Press();
            _timeButton.Press();
            _timeButton.Press();

            _startCancelButton.Press();

            _fakeTimer.Received().Start(Arg.Is(120));
        }

        [Test]
        public void UI_CookControllerStarted_PowerTubeReceivedCorrectValue()
        {
            _powerButton.Press();
            _powerButton.Press();
            _timeButton.Press();

            _startCancelButton.Press();

            _fakePowerTube.Received().TurnOn(Arg.Is(100));
        }


    }
}