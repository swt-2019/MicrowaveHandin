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
        public void UserInterface_ObservingDoor_LightOnWhenDoorOpen()
        {
            _doorToIntegrate.Open();

            _fakeLight.Received().TurnOn();
        }



        [Test]
        public void UserInterface_ObservingDoor_PowerDisplayClearWhenDoorOpen()
        {
            _fakePowerButton.Pressed += Raise.Event();

            _doorToIntegrate.Open();

            _fakeDisplay.Received().Clear();
        }

        [Test]
        public void UserInterface_ObservingDoor_TimeDisplayClearWhenDoorOpen()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();

            _doorToIntegrate.Open();

            _fakeDisplay.Received().Clear();
        }


        [Test]
        public void UserInterface_ObservingDoor_CookingStoppedWhenDoorOpen()
        {
            _fakePowerButton.Pressed += Raise.Event();
            _fakeTimeButton.Pressed += Raise.Event();
            _fakeStartCancelButton.Pressed += Raise.Event();

            _doorToIntegrate.Open();

            _fakeCookController.Received().Stop();
        }

        [Test]
        public void UserInterface_ObservingDoor_LightOffWhenDoorClosed()
        {
            _doorToIntegrate.Open();

            _doorToIntegrate.Close();

            _fakeLight.Received().TurnOff();
        }
    }
}