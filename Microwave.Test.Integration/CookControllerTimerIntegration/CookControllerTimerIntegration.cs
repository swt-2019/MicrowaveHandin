using System;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

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
        public void CookController_Timer_DefaultTimeIs60Seconds()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            
            Assert.AreEqual(60000,_timerToIntegrate.TimeRemaining);
        }
        
        [Test]
        public void CookController_Timer_TimeoutDefaultTime()
        {
            var isDone = false;
            _timerToIntegrate.Expired += (sender, args) => isDone = true;
            
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();
            
            Thread.Sleep(60050);
            
            Assert.IsTrue(isDone);
            
        }

        [TestCase(2)]
        [TestCase(5)]
        public void CookController_Timer_TimeOutCustomTimerDone(int customTime)
        {
            var isDone = false;
            _timerToIntegrate.Expired += (sender, args) => isDone = true;
            
            _topCookController.StartCooking(10,customTime);
            
            Thread.Sleep((customTime * 1000) + 10);
            
            Assert.IsTrue(isDone);
              
        }
        
        
        [TestCase(2)]
        [TestCase(5)]
        public void CookController_Timer_TimeOutCustomTimerNotDone(int customTime)
        {
            var isDone = false;
            _timerToIntegrate.Expired += (sender, args) => isDone = true;
            
            _topCookController.StartCooking(10,customTime);
            
            Thread.Sleep((customTime * 1000) - 10);
            
            Assert.IsFalse(isDone);
              
        }
        
        
    }
}