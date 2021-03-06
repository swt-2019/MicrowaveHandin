﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using MicrowaveOvenClasses.Boundary;
using MicrowaveOvenClasses.Controllers;
using MicrowaveOvenClasses.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace Microwave.Test.Integration.UI_LightAndDisplay
{
    public class ButtonLight
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

            light = new Light(output);

            display = new Display(output);
            cooker = Substitute.For<ICookController>();

            userInterface = new UserInterface(
                powerButtonTop, timeButtonTop, startCancelButtonTop,
                door,
                display,
                light,
                cooker);

        }

        [Test]
        public void Ready_DoorOpen_LightOn()
        {
            door.Open();
            output.Received().OutputLine(Arg.Is<string>(str => str.Contains("on")));
        }

        [Test]
        public void Ready_DoorClose_LightOff()
        {
            door.Open();
            door.Close();
            output.Received(1).OutputLine(Arg.Is<string>(str => str.Contains("off")));
        }

        [Test]
        public void Ready_DoorMultipleOpenClose_LightOff()
        {
            for (int i = 0; i < 100; i++)
            {
                door.Open();
                door.Close();
            }
            output.Received(100).OutputLine(Arg.Is<string>(str => str.Contains("off")));
        }
        
        [Test]
        public void Cooking_MultipleCookingIsDone_LightOff()
        {
            for (int i = 0; i < 100; i++)
            {
                powerButtonTop.Press();
                // Now in SetPower
                timeButtonTop.Press();
                // Now in SetTime
                startCancelButtonTop.Press(); // clear display
                // Now in cooking

                userInterface.CookingIsDone(); // clear display
            }

            output.Received(200).OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));
        }

        [Test]
        public void Cooking_CancelButton_CookerCalled()
        {
            powerButtonTop.Press();
            // Now in SetPower
            timeButtonTop.Press();
            // Now in SetTime
            startCancelButtonTop.Press();// clear display
            // Now in cooking
            
            
            startCancelButtonTop.Press();
            // stop cooking, clear display

            output.Received(2).OutputLine(Arg.Is<string>(str => str.Contains("Display cleared")));
        }

    }
}
