using System;
using System.Web.UI.WebControls;

namespace ch.appl.psoft.Common
{
    /// <summary>
    /// Summary description for BitsetCtrl1.
    /// </summary>
    public class BitsetCtrl : Table
	{
        public enum Mode {
            BitMask,
            BitNumber
        }

        private int _columnWidth = 0;
        private int _numberOfGroupItems = 0;
        private RepeatDirection _direction = RepeatDirection.Vertical;
        private CheckBox[] _items = new CheckBox[32];
        private int _highBit = -1;
        private bool _radio = false;
        private string _group = "";

        protected Mode _mode = Mode.BitMask;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="radio"></param>
        protected BitsetCtrl(Mode mode,bool radio) {
            _mode = mode;
            _radio = radio;
            init();
        }

        /// <summary>
        /// Get Mode
        /// </summary>
        public Mode mode {
            get { return _mode; }
        }

        /// <summary>
        /// Getb radio
        /// </summary>
        public bool radio {
            get { return _radio; }
        }

        /// <summary>
        /// Get/set repeat direction
        /// </summary>
        public RepeatDirection repeatDirection {
            set { _direction = value; }
            get { return _direction; }
        }

        /// <summary>
        /// Get/set column width
        /// </summary>
        public int columnWidth {
            set { _columnWidth = value; }
            get { return _columnWidth; }
        }

        /// <summary>
        /// Get/set number of Items in group (row or column)
        /// </summary>
        public int numberOfGroupItems {
            set { _numberOfGroupItems = value; }
            get { return _numberOfGroupItems; }
        }

        /// <summary>
        /// Get checkbox items
        /// </summary>
        public CheckBox[] items {
            get { return _items; }
        }

        /// <summary>
        /// Set bit
        /// </summary>
        /// <param name="bitNumber">bit nummber from 0 to 31</param>
        /// <param name="title"></param>
        /// <param name="selected"></param>
        public void setBit(int bitNumber, string title, bool selected) {
            CheckBox cb = _radio ? new RadioButton() : new CheckBox();
            if (_radio) ((RadioButton) cb).GroupName = _group;
            cb.Checked = selected;
            cb.Text = title;
            _items[bitNumber] = cb;
            _highBit = Math.Max(_highBit,bitNumber);
        }

        /// <summary>
        /// Get highest bit
        /// </summary>
        public int highBit {
            get { return _highBit; }
        }

        private void init() {
            base.Load += new System.EventHandler(this.onLoad);
            base.BorderWidth = 0;
            base.CellPadding = 0;
            base.CellSpacing = 0;
            if (_radio) _group = DateTime.Now.Ticks.ToString();
        }

        private void onLoad(object sender, System.EventArgs args) {
            TableRow r = null;
            TableCell c = null;

            if (_direction == RepeatDirection.Horizontal) {
                r = new TableRow();
                base.Rows.Add(r);
            }

            for (int i=0; i<=_highBit; i++) {
                if (_numberOfGroupItems > 0 && (i % _numberOfGroupItems) == 0 &&_direction == RepeatDirection.Horizontal) {
                    r = new TableRow();
                    base.Rows.Add(r);
                }
                if (_items[i] != null) {
                    c = new TableCell();
                    c.Wrap = false;
                    if (_direction == RepeatDirection.Vertical) {
                        r = null;
                        if (base.Rows.Count > 0 && _numberOfGroupItems > 0 && (i % _numberOfGroupItems) == 0) {
                            int idx = i / _numberOfGroupItems;
                            r = base.Rows[idx];
                        }
                        if (r == null) {
                            r = new TableRow();
                            base.Rows.Add(r);
                        }
                    }
                    c.Controls.Add(items[i]);
                    r.Cells.Add(c);
                    if (_columnWidth > 0) items[i].Width = _columnWidth;
                }
            }
        }



    }
    
    public class BitsetMapCtrl : BitsetCtrl {
        public BitsetMapCtrl() : base(Mode.BitMask,false) {
        }
    }
    public class BitsetNumberCtrl : BitsetCtrl {
        public BitsetNumberCtrl() : base(Mode.BitNumber,false) {
        }
    }
    public class BitsetRadioMapCtrl : BitsetCtrl {
        public BitsetRadioMapCtrl() : base(Mode.BitMask,true) {
        }
    }
    public class BitsetRadioNumberCtrl : BitsetCtrl {
        public BitsetRadioNumberCtrl() : base(Mode.BitNumber,true) {
        }
    }
    
}
