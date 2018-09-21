using ch.appl.psoft.db;
using ch.psoft.Util;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace ch.appl.psoft.Interface.DBObjects
{

    public class WikiImage : DBObject {
        public const string _TABLENAME = "WIKI_IMAGE";
        private const string _IMAGES_LINK = "Images/";
        private const string _THUMBS_LINK = "Thumbs/";
        public const int _MAX_THUMB_WIDTH = 80;
        public const int _MAX_THUMB_HEIGHT = 80;

        #region Properties

        private string imagesPath = "";
        public string ImagesPath
        {
            get { return imagesPath; }

            set
            {
                imagesPath = value;
            }
        }

        private string thumbsPath = "";
        public string ThumbsPath
        {
            get { return thumbsPath; }
            set { thumbsPath = value; }
        }

        public string ImagesURL
        {
            get
            {
                return Global.Config.baseURL + "/Wiki/" + _IMAGES_LINK;
            }
        }


        public string ThumbsURL
        {
            get
            {
                return Global.Config.baseURL + "/Wiki/" + _THUMBS_LINK;
            }
        }

        #endregion

        public WikiImage(DBData db, HttpSessionState session) : base(db, session) {
            imagesPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Wiki/" + _IMAGES_LINK;
            thumbsPath = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Wiki/" + _THUMBS_LINK;
        }

        private Image saveAsThumb(string sourceFilename, string targetFilename){
            Image sourceImage = Image.FromFile(sourceFilename);
            int thumbWidth = sourceImage.Width;
            int thumbHeight = sourceImage.Height;
            if (thumbWidth > _MAX_THUMB_WIDTH){
                if (thumbWidth > thumbHeight)
                {
                    double ratio = (double)_MAX_THUMB_WIDTH / (double)thumbWidth;
                    thumbHeight = (int)(ratio * thumbHeight);
                    thumbWidth = _MAX_THUMB_WIDTH;
                }
                else
                {
                    double ratio = (double)_MAX_THUMB_HEIGHT / (double)thumbHeight;
                    thumbWidth = (int)(ratio * thumbWidth);
                    thumbHeight = _MAX_THUMB_HEIGHT;
                }
            }
            else if(thumbHeight > _MAX_THUMB_HEIGHT)
            {
                if (thumbHeight > thumbWidth)
                {
                    double ratio = (double)_MAX_THUMB_HEIGHT / (double)thumbHeight;
                    thumbWidth = (int)(ratio * thumbWidth);
                    thumbHeight = _MAX_THUMB_HEIGHT;                   
                }
                else
                {
                    double ratio = (double)_MAX_THUMB_WIDTH / (double)thumbWidth;
                    thumbHeight = (int)(ratio * thumbHeight);
                    thumbWidth = _MAX_THUMB_WIDTH;
                }
            }

            Image result = sourceImage.GetThumbnailImage(_MAX_THUMB_WIDTH, _MAX_THUMB_HEIGHT, null, IntPtr.Zero);
            Image thumbImage = sourceImage.GetThumbnailImage(thumbWidth, thumbHeight, null, IntPtr.Zero);
            
            Graphics g = Graphics.FromImage(result);

            int positionX = (_MAX_THUMB_WIDTH - thumbWidth) / 2;
            int positionY = (_MAX_THUMB_HEIGHT - thumbHeight) / 2;

            g.FillRectangle(Brushes.White, 0, 0, _MAX_THUMB_WIDTH, _MAX_THUMB_HEIGHT);
            g.DrawImage(thumbImage,positionX,positionY,thumbWidth,thumbHeight);
            
            result.Save(targetFilename);
            return result;
        }

        private void deleteFile(string filename){
            try {
                File.Delete(filename);
            }
            catch(Exception ex) {
                Logger.Log(ex, Logger.ERROR);
            }
        }

        public override int delete(long ID, bool cascade) {
            string fileName = _db.lookup("FILENAME", _TABLENAME, "ID=" + ID, false);
            if (fileName != "") {
                deleteFile(ImagesPath + fileName);
                deleteFile(ThumbsPath + fileName);
            }

            return _db.execute("delete from " + _TABLENAME + " where ID=" + ID);
        }

        public int deleteAllToBeDeleted() {
            int retValue = 0;

            DataTable table = _db.getDataTable("select ID from " + _TABLENAME + " where TO_DELETE=1");
            foreach (DataRow row in table.Rows) {
                retValue += delete(ch.psoft.Util.Validate.GetValid(row["ID"].ToString(), -1), true);
            }

            return retValue;
        }

        public long addPicture(HttpPostedFile postedFile, string title, string description, long ownerUID) {
            long imageID = -1;
            if (postedFile != null && postedFile.FileName != "") {
                imageID = _db.newId(_TABLENAME);
                string fileName = imageID.ToString() + "_" + Path.GetFileName(postedFile.FileName);
                postedFile.SaveAs(ImagesPath + fileName);
                Image thumbImage = saveAsThumb(ImagesPath + fileName, ThumbsPath + fileName);
                _db.execute("insert into " + _TABLENAME + " (ID, OWNER_UID, FILENAME, TITLE, DESCRIPTION, THUMB_WIDTH, THUMB_HEIGHT) values (" + imageID + "," + ownerUID + ",'" + fileName + "','" + DBColumn.toSql(title) + "','" + DBColumn.toSql(description) + "'," + thumbImage.Width + "," + thumbImage.Height + ")");
            }
            
            return imageID;
        }

        public string getThumbURL(long ownerUID, string title){
            string thumbURL = _db.lookup("FILENAME", _TABLENAME, "OWNER_UID=" + ownerUID + " and TITLE='" + DBColumn.toSql(title) + "'", false);
            if (thumbURL != ""){
                thumbURL = ThumbsURL + thumbURL;
            }
            return thumbURL;
        }

        public string getImageURL(long ownerUID, string title){
            string imageURL = _db.lookup("FILENAME", _TABLENAME, "OWNER_UID=" + ownerUID + " and TITLE='" + DBColumn.toSql(title) + "'", false);
            if (imageURL != ""){
                imageURL = ImagesURL + imageURL;
            }
            return imageURL;
        }

        public string getDescription(long ownerUID, string title){
            return _db.lookup("DESCRIPTION", _TABLENAME, "OWNER_UID=" + ownerUID + " and TITLE='" + DBColumn.toSql(title) + "'", false);
        }

        public int getThumbWidth(long ownerUID, string title){
            return ch.psoft.Util.Validate.GetValid(_db.lookup("THUMB_WIDTH", _TABLENAME, "OWNER_UID=" + ownerUID + " and TITLE='" + DBColumn.toSql(title) + "'", false), 0);
        }

		public void addReferenceToImage(long wikiImageId, long ownerUID)
		{
			long imageID = _db.newId(_TABLENAME);
			_db.execute("insert into " + _TABLENAME + " (ID,OWNER_UID, FILENAME, TITLE, DESCRIPTION, THUMB_WIDTH, THUMB_HEIGHT) SELECT " + imageID + ","+ ownerUID + ", FILENAME, TITLE, DESCRIPTION, THUMB_WIDTH, THUMB_HEIGHT FROM WIKI_IMAGE WHERE ID = " + wikiImageId);
		}

    }
}
