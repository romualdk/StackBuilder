﻿#region Using directives
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;

using log4net;

using Sharp3D.Math.Core;

using treeDiM.StackBuilder.Basics;
using treeDiM.StackBuilder.Graphics;
using treeDiM.StackBuilder.Engine;
#endregion

namespace treeDiM.StackBuilder.Desktop
{
    public partial class FormNewHAnalysis : Form, IDrawingContainer
    {
        #region Constructor
        public FormNewHAnalysis()
        {
            InitializeComponent();
        }
        public FormNewHAnalysis(Document doc, HAnalysis analysis)
        {
            InitializeComponent();
            _document = doc;
            _analysis = analysis;

            if (null == _analysis)
            {
                _analysis = new HAnalysisPallet(_document);
                _analysis.ID.SetNameDesc(doc.GetValidNewAnalysisName("HAnalysis"), string.Empty);
            }
        }
        #endregion

        #region Form override
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // initialize graphic control 
            graphCtrl.DrawingContainer = this;

            // handling content grid events
            _checkBoxEvent.Click += new EventHandler(OnDataModified);
            _numUpDownEvent.ValueChanged += new EventHandler(OnDataModified);
            FillContentGrid();

            // handling row change in solution grid
            gridSolutions.Selection.SelectionChanged += OnSolutionChanged;

            OnDataModified(this, null);
        }

        private void OnSolutionChanged(object sender, SourceGrid.RangeRegionChangedEventArgs e)
        {
            graphCtrl.Invalidate();
        }
        #endregion

        #region IDrawingContainer
        public void Draw(Graphics3DControl ctrl, Graphics3D graphics)
        {
            try
            {
                HSolution sol = SelectedSolution;
                if (null != sol)
                {
                    ViewerHSolution sv = new ViewerHSolution(sol);
                    sv.Draw(graphics, Transform3D.Identity);
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Grids
        private void FillContentGrid()
        {
            try
            {
                // remove existing rows
                gridContent.Rows.Clear();
                // viewColumnHeader
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader()
                {
                    Background = new DevAge.Drawing.VisualElements.ColumnHeader()
                    {
                        BackColor = Color.LightGray,
                        Border = DevAge.Drawing.RectangleBorder.NoBorder
                    },
                    ForeColor = Color.Black,
                    Font = new Font("Arial", 10, FontStyle.Regular),
                };
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
                // viewNormal
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // ***
                // set first row
                gridContent.BorderStyle = BorderStyle.FixedSingle;
                gridContent.ColumnsCount = 5;
                gridContent.FixedRows = 1;

                // header
                int iCol = 0;
                gridContent.Rows.Insert(0);
                gridContent[0, iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_NAME) { AutomaticSortEnabled = false, View = viewColumnHeader };
                gridContent[0, ++iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_NUMBER) { AutomaticSortEnabled = false, View = viewColumnHeader };
                gridContent[0, ++iCol] = new SourceGrid.Cells.ColumnHeader("X") { AutomaticSortEnabled = false, View = viewColumnHeader };
                gridContent[0, ++iCol] = new SourceGrid.Cells.ColumnHeader("Y") { AutomaticSortEnabled = false, View = viewColumnHeader };
                gridContent[0, ++iCol] = new SourceGrid.Cells.ColumnHeader("Z") { AutomaticSortEnabled = false, View = viewColumnHeader };

                // content
                int iIndex = 0;
                foreach (ContentItem ci in ContentItems)
                {
                    // insert row
                    gridContent.Rows.Insert(++iIndex);
                    iCol = 0;
                    // name
                    gridContent[iIndex, iCol] = new SourceGrid.Cells.Cell(ci.Pack.Name) { View = viewNormal, Tag = ci.Pack };
                    // number
                    gridContent[iIndex, ++iCol] = new SourceGrid.Cells.Cell("NumericUpDown") { View = viewNormal };
                    gridContent[iIndex, iCol] = new SourceGrid.Cells.Cell((int)ci.Number) { View = viewNormal };
                    SourceGrid.Cells.Editors.NumericUpDown l_NumericUpDownEditor = new SourceGrid.Cells.Editors.NumericUpDown(typeof(int), 50, 0, 1);
                    l_NumericUpDownEditor.SetEditValue((int)ci.Number);
                    gridContent[iIndex, iCol].Editor = l_NumericUpDownEditor;
                    gridContent[iIndex, iCol].AddController(_numUpDownEvent);
                    // orientation X
                    gridContent[iIndex, ++iCol] = new SourceGrid.Cells.CheckBox(null, ci.AllowOrientX);
                    gridContent[iIndex, iCol].AddController(_checkBoxEvent);
                    // orientation Y
                    gridContent[iIndex, ++iCol] = new SourceGrid.Cells.CheckBox(null, ci.AllowOrientY);
                    gridContent[iIndex, iCol].AddController(_checkBoxEvent);
                    // orientation Z
                    gridContent[iIndex, ++iCol] = new SourceGrid.Cells.CheckBox(null, ci.AllowOrientZ);
                    gridContent[iIndex, iCol].AddController(_checkBoxEvent);
                }

                gridContent.AutoSizeCells();
                gridContent.Columns.StretchToFit();
                gridContent.AutoStretchColumnsToFitWidth = true;
                gridContent.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }

        private void FillResultGrid()
        {
            try
            {
                // remove existing rows
                gridSolutions.Rows.Clear();
                // viewColumnHeader
                SourceGrid.Cells.Views.ColumnHeader viewColumnHeader = new SourceGrid.Cells.Views.ColumnHeader()
                {
                    Background = new DevAge.Drawing.VisualElements.ColumnHeader()
                    {
                        BackColor = Color.LightGray,
                        Border = DevAge.Drawing.RectangleBorder.NoBorder
                    },
                    ForeColor = Color.Black,
                    Font = new Font("Arial", 10, FontStyle.Regular),
                };
                viewColumnHeader.ElementSort.SortStyle = DevAge.Drawing.HeaderSortStyle.None;
                // viewNormal
                CellBackColorAlternate viewNormal = new CellBackColorAlternate(Color.LightBlue, Color.White);
                // set first row
                gridSolutions.BorderStyle = BorderStyle.FixedSingle;
                gridSolutions.ColumnsCount = 5;
                gridSolutions.FixedRows = 1;

                // header
                int iCol = 0;
                gridSolutions.Rows.Insert(0);
                gridSolutions[0, iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_ALGORITHM) { View = viewColumnHeader };
                gridSolutions[0, ++iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_LOADEDCASES) { View = viewColumnHeader };
                gridSolutions[0, ++iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_UNLOADEDCASES) { View = viewColumnHeader };
                gridSolutions[0, ++iCol] = new SourceGrid.Cells.ColumnHeader(Properties.Resources.ID_LOADEDVOLUMEPERCENTAGE) { View = viewColumnHeader };

                // solutions
                int iIndex = 0;
                foreach (HSolution sol in _solutions)
                {
                    // insert row
                    gridSolutions.Rows.Insert(++iIndex);
                    iCol = 0;
                    // name
                    gridSolutions[iIndex, iCol] = new SourceGrid.Cells.Cell(sol.Algorithm) { View = viewNormal };
                    gridSolutions[iIndex, ++iCol] = new SourceGrid.Cells.Cell(sol.LoadedCasesCount) { View = viewNormal };
                    gridSolutions[iIndex, ++iCol] = new SourceGrid.Cells.Cell(sol.UnloadedCasesCount) { View = viewNormal };
                    gridSolutions[iIndex, ++iCol] = new SourceGrid.Cells.Cell(sol.LoadedVolumePercentage) { View = viewNormal };
                }

                gridSolutions.AutoSizeCells();
                gridSolutions.Columns.StretchToFit();
                gridSolutions.AutoStretchColumnsToFitWidth = true;
                gridSolutions.Invalidate();

                // select first solution
                if (gridSolutions.RowsCount > 1)
                    gridSolutions.Selection.SelectRow(1, true);
                else
                    graphCtrl.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Computation
        protected virtual Vector3D DimContainer { get; }
        protected virtual HConstraintSet ConstraintSet { get; }

        protected void LoadContentItems()
        {
            if (null == _analysis)
                return;
            // initialise analysis
            _analysis.ClearContent();
            for (int iRow = 1; iRow < gridContent.RowsCount; ++iRow)
            {
                // get packable
                Packable p = gridContent[iRow, 0].Tag as Packable;
                // get number
                SourceGrid.Cells.Editors.NumericUpDown upDownEditor = gridContent[iRow, 1].Editor as SourceGrid.Cells.Editors.NumericUpDown;
                int number = (int)upDownEditor.GetEditedValue();
                SourceGrid.Cells.CheckBox checkBoxX = gridContent[iRow, 2] as SourceGrid.Cells.CheckBox;
                SourceGrid.Cells.CheckBox checkBoxY = gridContent[iRow, 3] as SourceGrid.Cells.CheckBox;
                SourceGrid.Cells.CheckBox checkBoxZ = gridContent[iRow, 4] as SourceGrid.Cells.CheckBox;
                // get orientation
                bool[] orientations = new bool[3] { (bool)checkBoxX.Value, (bool)checkBoxY.Value, (bool)checkBoxZ.Value };
                _analysis.AddContent(p, (uint)number, orientations);
            }
        }

        protected virtual void LoadContainer() {}

        protected void Compute()
        {
            if (null == _analysis) return;
            LoadContentItems();
            LoadContainer();
            _analysis.ConstraintSet = ConstraintSet;

            if (!_analysis.IsValid)
                return;
            try
            {
                HSolver solver = new HSolver();
                _solutions = solver.BuildSolutions(_analysis);
            }
            catch (InvalidOperationException ex)
            {
                _log.WarnFormat("Solver -> {0}", ex.Message);
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        #region Event handlers
        protected void OnDataModified(object sender, EventArgs e)
        {
            try
            {
                Compute();
                FillResultGrid();
                graphCtrl.Invalidate();
            }
            catch (Exception ex)
            {
                _log.Error(ex.ToString());
            }
        }
        #endregion

        private HSolution SelectedSolution
        {
            get
            {
                if (_solutions.Count > 0)
                {
                    SourceGrid.RangeRegion region = gridSolutions.Selection.GetSelectionRegion();
                    int[] indexes = region.GetRowsIndex();
                    // no selection -> exit
                    if (indexes.Length == 0)
                        return null;
                    return _solutions[indexes[0] - 1];
                }
                else
                    return null;
            }
        }

        #region Helpers
        private List<ContentItem> ContentItems
        {
            get
            {
                List<ContentItem> contentItems = new List<ContentItem>();
                foreach (BoxProperties boxProperties in _document.Bricks)
                {
                    bool[] orientations = new bool[] { true, true, true };
                    contentItems.Add(new ContentItem(boxProperties, 1, orientations));
                }
                return contentItems;
            }
        }
        #endregion

        #region Data members
        protected Document _document;
        protected HAnalysis _analysis;
        protected List<HSolution> _solutions = new List<HSolution>();
        protected List<ContentItem> _contentItems;
        protected static ILog _log = LogManager.GetLogger(typeof(FormNewHAnalysis));
        protected List<BoxProperties> lBoxes = new List<BoxProperties>();

        protected SourceGrid.Cells.Controllers.CustomEvents _checkBoxEvent = new SourceGrid.Cells.Controllers.CustomEvents();
        protected SourceGrid.Cells.Controllers.CustomEvents _numUpDownEvent = new SourceGrid.Cells.Controllers.CustomEvents();
        #endregion
    }
}