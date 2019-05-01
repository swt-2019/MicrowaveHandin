using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Timer = MicrowaveOvenClasses.Boundary.Timer;

namespace Microwave.Test.Integration.CookControllerOutputterIntegration
{
    public class CookContollerOutputterTest
    {
        private UserInterface _userInterface;
        private IDoor _door;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDisplay _display;
        private ILight _light;
        private CookController _topCookController;
        private IOutput _outputToIntegrate;
        private ITimer _timer;
        private IPowerTube _powerTube;


        [SetUp]
        public void IntegrationTestSetup()
        {
            _outputToIntegrate = new Output();

            _timer = new Timer();
            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();
            _door = new Door();
            _light = new Light(_outputToIntegrate);
            _display = new Display(_outputToIntegrate);
            _powerTube = new PowerTube(_outputToIntegrate);
            _topCookController = new CookController(
                _timer,
                _display,
                _powerTube);
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
        [TestCase(3)]
        [TestCase(14)]
        public void CookController_OutputCorrectWhenSettingPower(int times)
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                string expected = "";
                for (int i = 0; i < times; i++)
                {
                    expected += $"Display shows: {(i+1)*50} W\r\n";
                }


                for (int i = 0; i < times; i++)
                {
                    _powerButton.Press();
                }


                Assert.AreEqual(expected, sw.ToString());
            }
        }
        
        [Test]
        public void CookController_OutputCorrectWhenSettingPowerOverMax()
        {
            using (StringWriter sw = new StringWriter())
            {
                var numberOfPresses = 15;
                Console.SetOut(sw);
                string expected = "";
                for (int i = 1; i <= 14; i++)
                {
                    expected += $"Display shows: {(((i))*50)} W\r\n";
                }
                expected += $"Display shows: 50 W\r\n";


                for (int i = 0; i < numberOfPresses; i++)
                {
                    _powerButton.Press();
                }


                Assert.AreEqual(expected, sw.ToString());
            }
        }
        
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(10)]
        public void CookController_OutputCorrectWhenSettingCookTime(int time)
        {
            using (StringWriter sw = new StringWriter())
            {
                Console.SetOut(sw);
                var expected = $"Display shows: 50 W\r\n";
                for (int c = 0; c < time; c++)
                {
                    expected += $"Display shows: {(c + 1):D2}:{0:D2}\r\n";
                }


                _powerButton.Press();
                for (int i = 0; i < time; i++)
                {
                    _timeButton.Press();
                }


                Assert.AreEqual(expected, sw.ToString());
            }
        }
        
        [Test]
        public void CookController_OutputCorrectWhenCookingDone()
        {
            using (StringWriter stringWriter = new StringWriter())
            {

                _powerButton.Press();
                _timeButton.Press();
                _startCancelButton.Press();
                
                Console.SetOut(stringWriter);

                Thread.Sleep(61000);
                
                Assert.True(stringWriter.ToString().Contains("Display cleared"));
            }
        }
        
        
        [Test]
        public void CookController_OutputCorrectWhenLightOn()
        {
            using (StringWriter stringWriter = new StringWriter())
            {
                Console.SetOut(stringWriter);
                
                _powerButton.Press();
                _timeButton.Press();
                _startCancelButton.Press();
                Thread.Sleep(1100);

                Assert.True(stringWriter.ToString().Contains("Light is turned on"));
            }
        }
    }
}