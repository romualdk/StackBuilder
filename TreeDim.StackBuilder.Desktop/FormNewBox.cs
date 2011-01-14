﻿#region Using directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using log4net;

using TreeDim.StackBuilder.Basics;
using TreeDim.StackBuilder.Graphics;
using Sharp3D.Math.Core;

using TreeDim.StackBuilder.Desktop.Properties;
#endregion

namespace TreeDim.StackBuilder.Desktop
{
    public partial class FormNewBox : Form
    {
        #region Mode enum
        public enum Mode
        { 
            MODE_BOX
            , MODE_CASE
        }
        #endregion

        #region Data members
        private Document _document;
        public Color[] _faceColors = new Color[6];
        public BoxProperties _boxProperties;
        public Mode _mode;
        static readonly ILog _log = LogManager.GetLogger(typeof(FormNewBox));
        #endregion

        #region Constructor
        /// <summary>
        /// FormNewBox constructor used when defining a new BoxProperties item
        /// </summary>
        /// <param name="document">Document in which the BoxProperties item is to be created</param>
        public FormNewBox(Document document, Mode mode)
        {
            InitializeComponent();
            // save document reference
            _document = document;
            // mode
            _mode = mode;
            switch (_mode)
            {
                case Mode.MODE_CASE:
                    nudLength.Value = 400.0M;
                    nudWidth.Value = 300.0M;
                    nudHeight.Value = 200.0M;
                    nudInsideLength.Value = nudLength.Value - 6.0M;
                    nudInsideWidth.Value = nudWidth.Value - 6.0M;
                    nudInsideHeight.Value = nudHeight.Value - 6.0M;
                    break;
                case Mode.MODE_BOX:
                    nudLength.Value = 120.0M;
                    nudWidth.Value = 60.0M;
                    nudHeight.Value = 30.0M;
                    nudInsideLength.Value = nudLength.Value - 6.0M;
                    nudInsideWidth.Value = nudWidth.Value - 6.0M;
                    nudInsideHeight.Value = nudHeight.Value - 6.0M;
                    break;
                default:
                    break;
            }
            // color : all faces set together / face by face
            chkAllFaces.Checked = false;
            chkAllFaces_CheckedChanged(this, null);
            // set colors
            for (int i=0; i<6; ++i)
                _faceColors[i] = _mode == Mode.MODE_BOX ? Color.Turquoise : Color.Chocolate;
            // set default face
            cbFace.SelectedIndex = 0;
            // set horizontal angle
            trackBarHorizAngle.Value = 45;
            // disable Ok button
            UpdateButtonOkStatus();
        }
        /// <summary>
        /// FormNewBox constructor used to edit existing boxes
        /// </summary>
        /// <param name="document">Document that contains the edited box</param>
        /// <param name="boxProperties">Edited box</param>
        public FormNewBox(Document document, BoxProperties boxProperties)
        {
            InitializeComponent();
            // save document reference
            _document = document;
            _boxProperties = boxProperties;
            _mode = boxProperties.HasInsideDimensions ? Mode.MODE_CASE : Mode.MODE_BOX;
            // set caption text
            Text = string.Format("Edit {0}...", _boxProperties.Name);
            // initialize value
            tbName.Text = _boxProperties.Name;
            tbDescription.Text = _boxProperties.Description;
            nudLength.Value = (decimal)_boxProperties.Length;
            nudInsideLength.Value = (decimal)_boxProperties.InsideLength;
            nudWidth.Value = (decimal)_boxProperties.Width;
            nudInsideWidth.Value = (decimal)_boxProperties.InsideWidth;
            nudHeight.Value = (decimal)_boxProperties.Height;
            nudInsideHeight.Value = (decimal)_boxProperties.InsideHeight;
            nudWeight.Value = (decimal)_boxProperties.Weight;
            nudWeightOnTop.Value = (decimal)0.0;
            // color : all faces set together / face by face
            chkAllFaces.Checked = _boxProperties.UniqueColor;
            chkAllFaces_CheckedChanged(this, null);
            // set colors
            for (int i=0; i<6; ++i)
                _faceColors[i] = _boxProperties.Colors[i];
            // set default face
            cbFace.SelectedIndex = 0;
            // set horizontal angle
            trackBarHorizAngle.Value = 45;
            // disable Ok button
            UpdateButtonOkStatus();
        }
        #endregion

        #region Public properties
        public string BoxName
        {
            get { return tbName.Text; }
            set { tbName.Text = value; }
        }
        public string Description
        {
            get { return tbDescription.Text; }
            set { tbDescription.Text = value; }
        }
        public double BoxLength
        {
            get { return (double)nudLength.Value; }
            set { nudLength.Value = (decimal)value; }
        }
        public double BoxWidth
        {
            get { return (double)nudWidth.Value; }
            set { nudWidth.Value = (decimal)value; }
        }
        public double BoxHeight
        {
            get { return (double)nudHeight.Value; }
            set { nudHeight.Value = (decimal)value; }
        }
        public double InsideLength
        {
            get { return (double)nudInsideLength.Value; }
            set { nudInsideLength.Value = (decimal)value; }
        }
        public double InsideWidth
        {
            get { return (double)nudInsideWidth.Value; }
            set { nudInsideWidth.Value = (decimal)value; }
        }
        public double InsideHeight
        {
            get { return (double)nudInsideHeight.Value; }
            set { nudInsideHeight.Value = (decimal)value; }
        }
        public double Weight
        {
            get { return (double)nudWeight.Value; }
            set { nudWeight.Value = (decimal)value; }
        }
        public double WeightOnTop
        {
            get { return (double)nudWeightOnTop.Value; }
            set { nudWeightOnTop.Value = (decimal)value; }
        }
        public Color[] Colors
        {
            get { return _faceColors; }
            set { }
        }
        #endregion

        #region Load / FormClosing event
        private void FormNewBox_Load(object sender, EventArgs e)
        {
            // show hide inside dimensions controls
            nudInsideLength.Visible = _mode == Mode.MODE_CASE;
            nudInsideWidth.Visible = _mode == Mode.MODE_CASE;
            nudInsideHeight.Visible = _mode == Mode.MODE_CASE;
            lbInsideLength.Visible = _mode == Mode.MODE_CASE;
            lbInsideWidth.Visible = _mode == Mode.MODE_CASE;
            lbInsideHeight.Visible = _mode == Mode.MODE_CASE;
            lbUnitLengthInside.Visible = _mode == Mode.MODE_CASE;
            lbUnitWidthInside.Visible = _mode == Mode.MODE_CASE;
            lbUnitHeightInside.Visible = _mode == Mode.MODE_CASE;
            lbWeightOnTop.Visible = _mode == Mode.MODE_CASE;
            nudWeightOnTop.Visible = _mode == Mode.MODE_CASE;
            lbUnitWeightOnTop.Visible = _mode == Mode.MODE_CASE;
            // caption
            this.Text = Mode.MODE_CASE == _mode ? Resources.ID_ADDNEWCASE : Resources.ID_ADDNEWBOX;
            // windows settings
            if (null != Settings.Default.FormNewBoxPosition)
                Settings.Default.FormNewBoxPosition.Restore(this);
            DrawBox();
        }
        private void FormNewBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            // window position
            if (null == Settings.Default.FormNewBoxPosition)
                Settings.Default.FormNewBoxPosition = new WindowSettings();
            Settings.Default.FormNewBoxPosition.Record(this);
        }
        #endregion

        #region Form override
        protected override void  OnResize(EventArgs e)
        {
            DrawBox();
 	         base.OnResize(e);
        }
        #endregion

        #region Handlers
        private void onBoxPropertyChanged(object sender, EventArgs e)
        {
            UpdateButtonOkStatus();
            DrawBox();
        }
        private void onSelectedFaceChanged(object sender, EventArgs e)
        {
            // get current index
            int iSel = cbFace.SelectedIndex;
            cbColor.Color = _faceColors[iSel];
            DrawBox();
        }
        private void onFaceColorChanged(object sender, EventArgs e)
        {
            if (!chkAllFaces.Checked)
            {
                int iSel = cbFace.SelectedIndex;
                _faceColors[iSel] = cbColor.Color;
            }
            else
            {
                for (int i = 0; i < 6; ++i)
                    _faceColors[i] = cbColor.Color;
            }
            DrawBox();
        }
        private void onHorizAngleChanged(object sender, EventArgs e)
        {
            DrawBox();
        }
        private void UpdateButtonOkStatus()
        {
            // status + message
            string message = string.Empty;
            if (string.IsNullOrEmpty(tbName.Text))
                message = Resources.ID_FIELDNAMEEMPTY;
            else if (string.IsNullOrEmpty(tbDescription.Text))
                message = Resources.ID_FIELDDESCRIPTIONEMPTY;
            else if (!_document.IsValidNewTypeName(tbName.Text, _boxProperties))
                message = string.Format(Resources.ID_INVALIDNAME, tbName.Text);
            else if (_mode == Mode.MODE_CASE && InsideLength > BoxLength)
                message = string.Format(Resources.ID_INVALIDINSIDELENGTH, InsideLength, BoxLength);
            else if (_mode == Mode.MODE_CASE && InsideWidth > BoxWidth)
                message = string.Format(Resources.ID_INVALIDINSIDEWIDTH, InsideWidth, BoxWidth);
            else if (_mode == Mode.MODE_CASE && InsideHeight > BoxHeight)
                message = string.Format(Resources.ID_INVALIDINSIDEHEIGHT, InsideHeight, BoxHeight);
            // accept
            bnOk.Enabled = string.IsNullOrEmpty(message);
            toolStripStatusLabelDef.ForeColor = string.IsNullOrEmpty(message) ? Color.Black : Color.Red;
            toolStripStatusLabelDef.Text = string.IsNullOrEmpty(message) ? Resources.ID_READY : message;
        }
        private void onNameDescriptionChanged(object sender, EventArgs e)
        {
            UpdateButtonOkStatus();
        }
        private void chkAllFaces_CheckedChanged(object sender, EventArgs e)
        {
            lbFace.Enabled = !chkAllFaces.Checked;
            cbFace.Enabled = !chkAllFaces.Checked;
            if (chkAllFaces.Checked)
                cbColor.Color = _faceColors[0];
        }
        private void btBitmaps_Click(object sender, EventArgs e)
        {
            FormEditBitmaps form = new FormEditBitmaps();
            form._boxProperties = _boxProperties;
            if (DialogResult.OK == form.ShowDialog())
            {
            }
        }
        #endregion

        #region Draw box
        private void DrawBox()
        {
            try
            {
                // get horizontal angle
                double angle = trackBarHorizAngle.Value;
                // instantiate graphics
                Graphics3DImage graphics = new Graphics3DImage(pictureBox.Size);
                graphics.CameraPosition = new Vector3D(
                    Math.Cos(angle * Math.PI / 180.0) * Math.Sqrt(2.0) * 10000.0
                    , Math.Sin(angle * Math.PI / 180.0) * Math.Sqrt(2.0) * 10000.0
                    , 10000.0);
                graphics.Target = Vector3D.Zero;
                graphics.LightDirection = new Vector3D(-0.75, -0.5, 1.0);
                graphics.SetViewport(-500.0f, -500.0f, 500.0f, 500.0f);
                // draw
                BoxProperties boxProperties = new BoxProperties(null, (double)nudLength.Value, (double)nudWidth.Value, (double)nudHeight.Value);
                boxProperties.SetAllColors(_faceColors);
                Box box = new Box(0, boxProperties);
                graphics.AddBox(box);
                graphics.AddDimensions(new DimensionCube((double)nudLength.Value, (double)nudWidth.Value, (double)nudHeight.Value));
                graphics.Flush();
                // set to picture box
                pictureBox.Image = graphics.Bitmap;
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString()); 
            }
        }
        #endregion
    }
}