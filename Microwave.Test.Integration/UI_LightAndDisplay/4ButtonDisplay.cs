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
                powerButtonTop, timeButtonTop, startCancelButtonTop, // buttons are subscribed in UserInterface
                door, // door is subscribed in UserInterface
                display,
                light,
                cooker);

            /*powerButtonTop.Pressed += new EventHandler(userInterface.OnPowerPressed);
            timeButtonTop.Pressed += new EventHandler(userInterface.OnTimePressed);
            startCancelButtonTop.Pressed += new EventHandler(userInterface.OnStartCancelPressed);*/

            /*door.Closed += new EventHandler(userInterface.OnDoorClosed);
            door.Opened += new EventHandler(userInterface.OnDoorOpened);*/

        }

        [Test]
        public void DisplayPowerButtonPress()
        {
            powerButtonTop.Press();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("50 W")));
        }

        [Test]
        public void DisplayTimeButtonPress_DefaultOneMinute()
        {
            powerButtonTop.Press();
            timeButtonTop.Press();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("01:00")));
        }

        [Test]
        public void DisplayTimeButtonPress_TwoTimesPress()
        {
            powerButtonTop.Press();
            timeButtonTop.Press();
            timeButtonTop.Press();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("02:00")));
        }

        [Test]
        public void DisplayTimeButtonPress_59TimesPress()
        {
            powerButtonTop.Press();
            for (var i = 0; i < 59; i++)
            {
                timeButtonTop.Press();
            }
            
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("59:00")));
        }

        [Test]
        public void DisplayTimeButtonPress_100TimesPress()
        {
            powerButtonTop.Press();
            for (var i = 0; i < 100; i++)
            {
                timeButtonTop.Press();
            }

            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("100:00")));
        }

        [Test]
        public void DisplayTimeButtonPress_100000TimesPress()
        {
            powerButtonTop.Press();
            for (var i = 0; i < 100000; i++)
            {
                timeButtonTop.Press();
            }

            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("100000:00")));
        }

        [Test]
        public void DisplayStartButtonPress()
        {
            powerButtonTop.Press();
            timeButtonTop.Press();
            startCancelButtonTop.Press();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));
        }

        [Test]
        public void DisplayTimeButtonPress_TwoTimesPressAndThenStart()
        {
            powerButtonTop.Press();
            timeButtonTop.Press();
            timeButtonTop.Press();
            startCancelButtonTop.Press();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));
        }

    }
}
