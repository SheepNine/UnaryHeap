using Patchwork.Properties;
using System;
using System.Text;

namespace Patchwork
{
    public interface ISettingsLocker
    {
        MruList LoadMruList();
        void SaveMruList(MruList data);

        string LoadCurrentArrangementFilename();
        void SaveCurrentArrangementFilename(string currentFileName);

        int LoadScale();
        void SaveScale(int value);

        bool LoadGridVisibility();
        void SaveGridVisibility(bool value);

        string LoadCurrentTilesetFilename();
        int LoadCurrentTilesetTileSize();
        void SaveCurrentTileset(string filename, int tileSize);
    }

    class SettingsLocker : ISettingsLocker
    {
        Settings backingStore;

        public SettingsLocker(Settings backingStore)
        {
            this.backingStore = backingStore;
        }

        public MruList LoadMruList()
        {
            return new MruList(backingStore.MruList.Split(
                new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
        }

        public void SaveMruList(MruList data)
        {
            var text = new StringBuilder();

            for (int i = 0; i < data.Count; i++)
            {
                if (0 != i)
                    text.Append('|');

                text.Append(data[i]);
            }

            backingStore.MruList = text.ToString();
        }

        public void Persist()
        {
            backingStore.Save();
        }

        public string LoadCurrentArrangementFilename()
        {
            return backingStore.LatestArrangementFilename;
        }

        public void SaveCurrentArrangementFilename(string filename)
        {
            backingStore.LatestArrangementFilename = filename;
        }

        public int LoadScale()
        {
            return backingStore.Scale;
        }

        public void SaveScale(int value)
        {
            backingStore.Scale = value;
        }

        public bool LoadGridVisibility()
        {
            return backingStore.GridVisible;
        }

        public void SaveGridVisibility(bool value)
        {
            backingStore.GridVisible = value;
        }

        public string LoadCurrentTilesetFilename()
        {
            return backingStore.LatestTilesetFilename;
        }

        public int LoadCurrentTilesetTileSize()
        {
            return backingStore.LatestTilesetTileSize;
        }

        public void SaveCurrentTileset(string filename, int tileSize)
        {
            backingStore.LatestTilesetFilename = filename;
            backingStore.LatestTilesetTileSize = tileSize;
        }
    }
}
