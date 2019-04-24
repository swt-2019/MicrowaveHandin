using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class CookControllerTimerIntegration
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
        private ITimer _timerToIntegrate;
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
            _timerToIntegrate = new Timer();
            
            _topCookController = new CookController(
                _timerToIntegrate,
                _display,
                _fakePowerTube);
            
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


        [Test]
        public void CookController_TimerStart_TimerCorrect()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            
            Assert.AreEqual(60,_timerToIntegrate.TimeRemaining);
        }
        
    }
}