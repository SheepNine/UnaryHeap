using Patchwork.Properties;
using System;
using System.Text;

namespace Patchwork
{
    public interface ISettingsLocker
    {
        MruList LoadMruList();
        void SaveMruList(MruList data);
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
    }
}
