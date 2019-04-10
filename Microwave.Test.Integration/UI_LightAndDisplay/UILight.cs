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


            output = new Output();

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
            // This test that uut has subscribed to door opened, and works correctly
            // simulating the event through NSubstitute
            door.Opened += Raise.EventWith(this, EventArgs.Empty);
            //Assert.That(() => uut.);
        }


    }
}
