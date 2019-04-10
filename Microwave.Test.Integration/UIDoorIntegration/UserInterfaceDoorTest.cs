using System;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration
{
    [TestFixture]
    public class UserInterfaceDoorTest
    {
        private IUserInterface _topUserInterface;
        private IDoor _doorToIntegrate;
        private IButton _fakePowerButton;
        private IButton _fakeTimeButton;
        private IButton _fakeStartCancelButton;
        private IDisplay _fakeDisplay;
        private ILight _fakeLight;
        private ICookController _fakeCookController;
        [SetUp]
        public void IntegrationTestSetup()
        {
            _fakePowerButton = Substitute.For<IButton>();
            _fakeTimeButton = Substitute.For<IButton>();
            _fakeStartCancelButton = Substitute.For<IButton>();
            _fakeDisplay = Substitute.For<IDisplay>();
            _fakeLight = Substitute.For<ILight>();
            _fakeCookController = Substitute.For<ICookController>();
            _doorToIntegrate = new Door();
            _topUserInterface = new UserInterface(
                _fakePowerButton,
                _fakeTimeButton,
                _fakeStartCancelButton,
                _doorToIntegrate,
                _fakeDisplay,
                _fakeLight,
                _fakeCookController);
        }

        [Test]
        public void UserInterface_ObservingDoor_()
        {
            _doorToIntegrate.Open();

            _fakeLight.Received().TurnOn();
        }
    }
}