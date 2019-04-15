using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Smtp;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit;
using NUnit.Framework;


namespace Microwave.Test.Integration.UIButtonIntegration
{
    public class UiButton
    {
        //testing between UI and buttons
        private UserInterface top_;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;

        private IDoor _door;
        private IDisplay _display;
        private ILight _light;
        private ICookController _cookController;
        private IOutput _fakeOutput;
        private ITimer _fakeTimer;
        private IPowerTube _fakePowerTube;



        [SetUp]
        public void Setup()
        {
            _cookController = Substitute.For<ICookController>();

            _powerButton = new Button();
            _timeButton = new Button();
            _startCancelButton = new Button();

            _door = new Door();
            _light = Substitute.For<ILight>();
            _display = Substitute.For<IDisplay>();
            top_ = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _cookController);

        }

        [Test]
        public void ui_btn_check_all_btnEvents()
        {
            bool powerNotified = false;
            bool timerNotified = false;
            bool startCancelNotified = false;

            _powerButton.Pressed += (sender, args) => powerNotified = true;
            _timeButton.Pressed += (sender, args) => timerNotified = true;
            _startCancelButton.Pressed += (sender, args) => startCancelNotified = true;

            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            Assert.That(powerNotified == true && timerNotified == true && startCancelNotified == true);
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(20)]
        [TestCase(0)]
        public void ui_btn_powerBtn(int timesPressed)
        {
            int power = 50;
            //set state to SetPower
            _powerButton.Press();
            for (int i = 0; i < timesPressed; i++)
            {
                _powerButton.Press();
                if (power >= 700)
                {
                    power = 50;
                }
                else
                {
                    power += 50;
                }
            }

             _display.Received().ShowPower(power);
            
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(11)]
        [TestCase(0)]
        public void ui_btn_timebtn(int timesPressed)
        {
            
            //set state
            _powerButton.Press();

            for (int i = 0; i < timesPressed; i++)
            {
                _timeButton.Press();
            }

            if (timesPressed > 0)
            {
                _display.Received().ShowTime(timesPressed, 0);
            }
            else
            {
                _display.DidNotReceive().ShowTime(1,0);
            }
        }

        [Test]
        public void ui_btn_StartCancelBtnPowerState()
        {
            //set State
            _powerButton.Press();

            _startCancelButton.Press();
            _light.Received().TurnOff();
        }
        [Test]
        public void ui_btn_StartCancelBtnTimeState()
        {
            //set State
            _powerButton.Press();
            _timeButton.Press();

            _startCancelButton.Press();
            _light.Received().TurnOn();
        }
        [Test]
        public void ui_btn_StartCancelBtnCookingState()
        {
            //set State
            _powerButton.Press();
            _timeButton.Press();

            _startCancelButton.Press();
            _startCancelButton.Press();
            _cookController.Received().Stop();
        }
    }
}
