﻿using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration.UICookControllerIntegration
{
    [TestFixture]
    public class CookControllerUserInterfaceIntegration
    {

        private IUserInterface _userInterfaceToIntegrate;
        private IDoor _door;
        private IButton _powerButton;
        private IButton _timeButton;
        private IButton _startCancelButton;
        private IDisplay _display;
        private ILight _light;
        private ICookController _topCookController;
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
            _userInterfaceToIntegrate = new UserInterface(
                _powerButton,
                _timeButton,
                _startCancelButton,
                _door,
                _display,
                _light,
                _topCookController);
            _topCookController = new CookController(
                _fakeTimer,
                _display,
                _fakePowerTube,
                _userInterfaceToIntegrate);
        }

        [Test]
        public void CookController_TimeExpired_DisplayCleared()
        {
            _powerButton.Press();
            _timeButton.Press();
            _startCancelButton.Press();

            _fakeTimer.Expired += Raise.Event();

            _fakeOutput.Received().OutputLine(Arg.Any<string>());
        }

        [Test]
        public void CookController_TimeExpired_LightOff()
        {
            _topCookController.StartCooking(50, 100);

            _fakeTimer.Expired += Raise.Event();

            _light.Received().TurnOff();
        }
    }
}