using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TGRFramework.Prototype.Common;
using Microsoft.Xna.Framework;

namespace TGRFramework.Prototype.LevelEditor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
            this.KeyUp += new KeyEventHandler(Form1_KeyUp);

            // Setup context menu
            this.contextMenu.Opened += new EventHandler(contextMenu_Opened);
            this.contextMenu.Closed += new ToolStripDropDownClosedEventHandler(contextMenu_Closed);

            // Setup trackbar
            trackBar1.Minimum = 1;
            trackBar1.LargeChange = 25;
            trackBar1.SmallChange = 5;
            trackBar1.Maximum = 250;
            trackBar1.TickFrequency = 25;
            trackBar1.Value = 75;
            this.levelEditorControl1.FormControlLocations.Add(new Microsoft.Xna.Framework.Rectangle(this.trackBar1.Location.X, this.trackBar1.Location.Y, this.trackBar1.Width, this.trackBar1.Height));

            // Setup combo box
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.MouseEnter += new EventHandler(comboBox1_MouseEnter);
            comboBox1.MouseLeave += new EventHandler(comboBox1_MouseLeave);
            comboBox1.DropDownClosed += new EventHandler(comboBox1_DropDownClosed);
            comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
            comboBox1.DropDown += new EventHandler(comboBox1_DropDown);
            comboBox1.Items.Add("Air");
            comboBox1.Items.Add("Underground");
            comboBox1.Items.Add("Dirt");
            comboBox1.SelectedIndex = 0;
            LevelEditorControl.ComboBoxItems = new string[comboBox1.Items.Count];
            for (int i = 0; i < comboBox1.Items.Count; i++) LevelEditorControl.ComboBoxItems[i] = comboBox1.Items[i].ToString();

            this.levelEditorControl1.FormControlLocations.Add(new Microsoft.Xna.Framework.Rectangle(this.comboBox1.Location.X, this.comboBox1.Location.Y, this.comboBox1.Width, this.comboBox1.Height));
            
            // Setup save button
            this.levelEditorControl1.FormControlLocations.Add(new Microsoft.Xna.Framework.Rectangle(this.button1.Location.X, this.button1.Location.Y, this.button1.Width, this.button1.Height));

        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            if (!LevelEditorControl.MenuOpen) // TODO - MouseEnter does not always fire
            {
                this.FreezeMouseState();
            }
        }

        private void comboBox1_MouseEnter(object sender, EventArgs e)
        {
            this.FreezeMouseState();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            LevelEditorControl.ComboBoxSelection = comboBox1.SelectedItem.ToString();
            this.trackBar1.Focus();
        }

        private void comboBox1_MouseLeave(object sender, EventArgs e) // TODO_LOW May unfreeze too early
        {
            if (!this.comboBox1.DroppedDown)
            {
                this.BeginInvoke(new Action(() => { this.UnfreezeMouseState(); }));
            }
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            this.BeginInvoke(new Action(() => { this.UnfreezeMouseState(); }));
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Space)
            {
                LevelEditorControl.DeleteKeyDown = false;
            }        
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            float scrollVelocity = (float)trackBar1.Value; // TODO configurable
            
            if (e.KeyCode == Keys.D)
            {             
                this.levelEditorControl1.ScrollX = Math.Min(this.levelEditorControl1.ScrollX + scrollVelocity, PlatformerLevel.LevelWidth - this.levelEditorControl1.GraphicsDevice.Viewport.Width);
            }
            else if (e.KeyCode == Keys.A)
            {
                this.levelEditorControl1.ScrollX = Math.Max(this.levelEditorControl1.ScrollX - scrollVelocity, 0);
            }
            else if (e.KeyCode == Keys.S)
            {
                this.levelEditorControl1.ScrollY = Math.Min(this.levelEditorControl1.ScrollY + scrollVelocity, PlatformerLevel.LevelHeight - this.levelEditorControl1.GraphicsDevice.Viewport.Height);
            }
            else if (e.KeyCode == Keys.W)
            {
                this.levelEditorControl1.ScrollY = Math.Max(this.levelEditorControl1.ScrollY - scrollVelocity, 0);
            }
            else if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Space)
            {
                LevelEditorControl.DeleteKeyDown = true;
            }
            else if (e.KeyCode == Keys.R)
            {
                this.comboBox1.SelectedIndex = this.comboBox1.SelectedIndex == this.comboBox1.Items.Count - 1 ? 0 : this.comboBox1.SelectedIndex + 1;
            }
        }

        private void contextMenu_Opened(object sender, EventArgs e)
        {
            this.FreezeMouseState();
        }

        private void contextMenu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            if (e.CloseReason != ToolStripDropDownCloseReason.ItemClicked)
            {
                this.UnfreezeMouseState();
            }
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.levelEditorControl1.LevelSprite.ClearSelectedLocations();
            this.UnfreezeMouseState();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.levelEditorControl1.LevelSprite.DeleteCurrentLocation();
            this.UnfreezeMouseState();
        }

        private void applyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.levelEditorControl1.LevelSprite.Apply();
            this.UnfreezeMouseState();
        }

        private void FreezeMouseState()
        {
            Microsoft.Xna.Framework.Input.MouseState currentMouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();

            // Freeze mouse state while context menu is up
            LevelEditorControl.MouseState = new Microsoft.Xna.Framework.Input.MouseState(currentMouseState.X, currentMouseState.Y, currentMouseState.ScrollWheelValue, currentMouseState.LeftButton, currentMouseState.MiddleButton, currentMouseState.RightButton, currentMouseState.XButton1, currentMouseState.XButton2);
            LevelEditorControl.MenuOpen = true;
        }

        private void UnfreezeMouseState()
        {
            LevelEditorControl.MouseState = default(Microsoft.Xna.Framework.Input.MouseState);
            LevelEditorControl.MenuOpen = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.levelEditorControl1.LevelSprite.Save("testLevel"); // TODO filename
        }
    }
}
