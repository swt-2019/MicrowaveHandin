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
    public class ButtonDisplay
    {

        //Setup and creation of fakes: stubs or mocks
        private UserInterface userInterface;

        private IButton powerButtonTop;
        private IButton timeButtonTop;
        private IButton startCancelButtonTop;

        private IDoor door;

        private IDisplay display;
        private ILight light;

        private ICookController cooker;

        private IOutput output;

        [SetUp]
        public void SetUp()
        {

            powerButtonTop = new Button();
            timeButtonTop = new Button();
            startCancelButtonTop = new Button();
            door = new Door();


            output = Substitute.For<IOutput>();

            light = Substitute.For<ILight>();

            display = new Display(output);
            cooker = Substitute.For<ICookController>();

            userInterface = new UserInterface(
                powerButtonTop, timeButtonTop, startCancelButtonTop,
                door,
                display,
                light,
                cooker);

            powerButtonTop.Pressed += new EventHandler(userInterface.OnPowerPressed);
            timeButtonTop.Pressed += new EventHandler(userInterface.OnTimePressed);
            startCancelButtonTop.Pressed += new EventHandler(userInterface.OnStartCancelPressed);

            door.Closed += new EventHandler(userInterface.OnDoorClosed);
            door.Opened += new EventHandler(userInterface.OnDoorOpened);

        }

        [Test]
        public void DisplayStartCookingClearDisplay()
        {
            display.Clear();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("cleared")));
        }

        [Test]
        public void DisplayPowerButtonPress()
        {
            powerButtonTop.Press();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("50 W")));
        }

        [Test]
        public void DisplayTimeButtonPress()
        {
            powerButtonTop.Press();
            timeButtonTop.Press();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("01:00")));
        }

        [Test]
        public void DisplayStartButtonPress()
        {
            powerButtonTop.Press();
            timeButtonTop.Press();
            startCancelButtonTop.Press();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("100 W")));
        }

        [Test]
        public void Something()
        {
            
        }

    }
}
