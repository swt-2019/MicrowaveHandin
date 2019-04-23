using System;
using System.Collections.Generic;
using System.Text;

using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration.UI_LightAndDisplay
{
    public class UILight
    {
        //Setup and creation of fakes: stubs or mocks
        private UserInterface uut;

        private IButton powerButton;
        private IButton timeButton;
        private IButton startCancelButton;

        private IDoor door;

        private IDisplay display;
        private ILight light;

        private ICookController cooker;

        private IOutput output;

        [SetUp]
        public void SetUp()
        {

            powerButton = new Button();
            timeButton = new Button();
            startCancelButton = new Button();
            door = new Door();


            output = Substitute.For<IOutput>();

            light = new Light(output);

            display = Substitute.For<IDisplay>();
            cooker = Substitute.For<ICookController>();

            uut = new UserInterface(
                powerButton, timeButton, startCancelButton,
                door,
                display,
                light,
                cooker);

            powerButton.Pressed += new EventHandler(uut.OnPowerPressed);
            timeButton.Pressed += new EventHandler(uut.OnTimePressed);
            startCancelButton.Pressed += new EventHandler(uut.OnStartCancelPressed);

            door.Closed += new EventHandler(uut.OnDoorClosed);
            door.Opened += new EventHandler(uut.OnDoorOpened);

        }

        [Test]
        public void Ready_DoorOpen_LightOn()
        {
            door.Open();
            output.Received().OutputLine(Arg.Is("Light is turned on"));
        }

        [Test]
        public void Ready_DoorClose_LightOff()
        {
            door.Open();
            door.Close();
            output.Received().OutputLine(Arg.Is("Light is turned off"));
        }

        [Test]
        public void Cooking_CookingIsDone_LightOff()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            uut.CookingIsDone();

            output.Received().OutputLine(Arg.Is("Light is turned off"));
        }

        [Test]
        public void Cooking_CancelButton_CookerCalled()
        {
            powerButton.Press();
            // Now in SetPower
            timeButton.Press();
            // Now in SetTime
            startCancelButton.Press();
            // Now in cooking

            // Open door
            startCancelButton.Press();

            output.Received().OutputLine(Arg.Is("Light is turned off"));
        }


    }
}
