using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class TimerCookControllerIntegration
    {
        private UserInterface _userInterface;
        private IDoor _door;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDisplay _display;
        private ILight _light;
        private CookController _cookControllerToIntegrate;
        private IOutput _fakeOutput;
        private ITimer _topTimer;
        private IPowerTube _fakePowerTube;
        
        
        [SetUp]
        public void IntegrationTestSetup()
        {
            _fakeOutput = Substitute.For<IOutput>();
            _fakePowerTube = Substitute.For<IPowerTube>();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_fakeOutput);
            _display = new Display(_fakeOutput);
            _topTimer = new Timer();
            
            _cookControllerToIntegrate = new CookController(
                _topTimer,
                _display,
                _fakePowerTube);
            
            _userInterface = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cookControllerToIntegrate);
            _cookControllerToIntegrate.UI = _userInterface;
        }

        [Test]
        public void Timer_CookController_TimeExpired_DisplayCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            
            Thread.Sleep(1010);
            
            _fakeOutput.Received().OutputLine("Display cleared");
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Timer_CookController_DisplaysCorrectTime(int time)
        {
            _powerButton.Press();
            for (int i = 0; i < time; i++)
            {
                _timeButton.Press();
            }
            _startCancelButton.Press();
            
            
            _fakeOutput.Received().OutputLine($"Display shows: {time:D2}:{0:D2}");
        }

        [TestCase(15)]
        [TestCase(30)]
        [TestCase(45)]
        public void Timer_CookController_DisplayShowsCorrectTimeAfterSetDuration(int duration)
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            var secondsRemaining = 60 - duration;
            
            Thread.Sleep((duration*1000) + 150);
            
            _fakeOutput.Received().OutputLine($"Display shows: {0:D2}:{secondsRemaining:D2}");
        }
        
    }
}