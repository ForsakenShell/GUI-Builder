using System;
using System.Runtime.InteropServices;

namespace XeLib.Internal
{
    //internal static class Functions
    public static class Functions
    {
        const string DllPath = @"XEditLib.dll";

        // META METHODS
        //void (__cdecl* InitXEdit) ();
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitXEdit();

        //void (__cdecl* CloseXEdit) ();
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void CloseXEdit();

        //WordBool(__cdecl* GetResultString)(PWChar, Integer);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetResultString(Byte[] result, int len);

        //WordBool(__cdecl* GetResultArray)(PCardinal, Integer);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetResultArray(uint[] result, int len);

        //WordBool(__cdecl* GetGlobal)(PWChar, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetGlobal(string key, out int len);

        //WordBool(__cdecl* GetGlobals)(PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetGlobals(out int len);

        //WordBool(__cdecl* SetSortMode)(Byte, WordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetSortMode(Byte mode, bool reverse);

        //WordBool(__cdecl* Release)(Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Release(uint handle);

        //WordBool(__cdecl* ReleaseNodes)(Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ReleaseNodes(uint handle);

        //WordBool(__cdecl* Switch)(Cardinal, Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Switch(uint handleOne, uint handleTwo);

        //WordBool(__cdecl* GetDuplicateHandles)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetDuplicateHandles(uint handle, out int len);

        //WordBool(__cdecl* ResetStore)();
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ResetStore();

        // MESSAGE METHODS
        //void (__cdecl* GetMessagesLength) (PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetMessagesLength(out int len);

        //WordBool(__cdecl* GetMessages)(PWChar, Integer);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetMessages(Byte[] result, int len);

        //void (__cdecl* ClearMessages) ();
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void ClearMessages();

        //void (__cdecl* GetExceptionMessageLength) (PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern void GetExceptionMessageLength(out int len);

        //WordBool(__cdecl* GetExceptionMessage)(PWChar, Integer);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetExceptionMessage(Byte[] result, int len);

        // LOADING AND SET UP METHODS
        //WordBool(__cdecl* SetGamePath)(PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetGamePath(string gamePath);

        //WordBool(__cdecl* SetLanguage)(PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetLanguage(string language);

        //WordBool(__cdecl* SetBackupPath)(PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetBackupPath(string backupPath);

        //WordBool(__cdecl* SetGameMode)(Integer);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetGameMode(int gameMode);

        //WordBool(__cdecl* GetGamePath)(Integer, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetGamePath(int gameMode, out int len);

        //WordBool(__cdecl* GetLoadOrder)(PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetLoadOrder(out int len);

        //WordBool(__cdecl* GetActivePlugins)(PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetActivePlugins(out int len);

        //WordBool(__cdecl* LoadPlugins)(PWChar, WordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LoadPlugins(string plugins, bool smartLoad);

        //WordBool(__cdecl* LoadPlugin)(PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LoadPlugin(string filename);

        //WordBool(__cdecl* LoadPluginHeader)(PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LoadPluginHeader(string filename, out uint handle);

        //WordBool(__cdecl* BuildReferences)(Cardinal, WordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool BuildReferences(uint handle, bool sync);

        //WordBool(__cdecl* GetLoaderStatus)(PByte);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetLoaderStatus(out Byte state);

        //WordBool(__cdecl* UnloadPlugin)(Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool UnloadPlugin(uint handle);

        // FILE HANDLING METHODS
        //WordBool(__cdecl* AddFile)(PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddFile(string filename, out uint handle);

        //WordBool(__cdecl* FileByIndex)(Integer, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FileByIndex(int index, out uint handle);

        //WordBool(__cdecl* FileByLoadOrder)(Integer, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FileByLoadOrder(int loadOrder, out uint handle);

        //WordBool(__cdecl* FileByName)(PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FileByName(string filename, out uint handle);

        //WordBool(__cdecl* FileByAuthor)(PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FileByAuthor(string author, out uint handle);

        //WordBool(__cdecl* NukeFile)(Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool NukeFile(uint handle);

        //WordBool(__cdecl* RenameFile)(Cardinal, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RenameFile(uint handle, string filename);

        //WordBool(__cdecl* SaveFile)(Cardinal, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SaveFile(uint handle, string filename);

        //WordBool(__cdecl* GetRecordCount)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetRecordCount(uint handle, out int count);

        //WordBool(__cdecl* GetOverrideRecordCount)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetOverrideRecountCount(uint handle, out int count);

        //WordBool(__cdecl* MD5Hash)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MD5Hash(uint handle, out int len);

        //WordBool(__cdecl* CRCHash)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CRCHash(uint handle, out int len);

        //WordBool(__cdecl* SortEditorIDs)(Cardinal, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SortEditorIDs(uint handle, string signature);

        //WordBool(__cdecl* SortNames)(Cardinal, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SortNames(uint handle, string signature);

        //WordBool(__cdecl* GetFileLoadOrder)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetFileLoadOrder(uint handle, out uint loadOrder);

        // MASTER HANDLING METHODS
        //WordBool(__cdecl* CleanMasters)(Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CleanMasters(uint handle);

        //WordBool(__cdecl* SortMasters)(Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SortMasters(uint handle);

        //WordBool(__cdecl* AddMaster)(Cardinal, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddMaster(uint handle, string filename);

        //WordBool(__cdecl* AddMasters)(Cardinal, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddMasters(uint handle, string masters);

        //WordBool(__cdecl* AddRequiredMasters)(Cardinal, Cardinal, WordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddRequiredMasters(uint handleOne, uint handleTwo, bool asNew);

        //WordBool(__cdecl* GetMasters)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetMasters(uint handle, out int len);

        //WordBool(__cdecl* GetRequiredBy)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetRequiredBy(uint handle, out int len);

        //WordBool(__cdecl* GetMasterNames)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetMasterNames(uint handle, out int len);

        // ELEMENT HANDLING METHODS
        //WordBool(__cdecl* HasElement)(Cardinal, PWChar, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool HasElement(uint element, string path, out bool res);

        //WordBool(__cdecl* GetElement)(Cardinal, PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetElement(uint element, string path, out uint res);

        //WordBool(__cdecl* AddElement)(Cardinal, PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddElement(uint element, string path, out uint res);

        //WordBool(__cdecl* AddElementValue)(Cardinal, PWChar, PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddElementValue(uint element, string path, string value, out uint res);

        //WordBool(__cdecl* RemoveElement)(Cardinal, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RemoveElement(uint element, string path);

        //WordBool(__cdecl* RemoveElementOrParent)(Cardinal, PWChar);
        // ORIGINAL ERROR: No path
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RemoveElementOrParent(uint element);

        //WordBool(__cdecl* SetElement)(Cardinal, Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetElement(uint elementOne, uint elementTwo);

        //WordBool(__cdecl* GetElements)(Cardinal, PWChar, WordBool, WordBool, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetElements(uint element, string path, bool sort, bool filter,
            out int len);

        //WordBool(__cdecl* GetDefNames)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetDefNames(uint element, out int len);

        //WordBool(__cdecl* GetAddList)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetAddList(uint element, out int len);

        //WordBool(__cdecl* GetContainer)(Cardinal, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetContainer(uint element, out uint res);

        //WordBool(__cdecl* GetElementFile)(Cardinal, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetElementFile(uint element, out uint res);

        // ORIGINAL ERROR: missing
        //WordBool(__cdecl* GetElementGroup)(Cardinal, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetElementGroup(uint element, out uint res);

        //WordBool(__cdecl* GetElementRecord)(Cardinal, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetElementRecord(uint element, out uint res);

        //WordBool(__cdecl* GetLinksTo)(Cardinal, PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetLinksTo(uint element, string path, out uint res);

        //WordBool(__cdecl* SetLinksTo)(Cardinal, PWChar, Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetLinksTo(uint elementOne, string path, uint elementTwo);

        //WordBool(__cdecl* ElementCount)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ElementCount(uint element, out int count);

        //WordBool(__cdecl* ElementEquals)(Cardinal, Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ElementEquals(uint elementOne, uint elementTwo, out bool res);

        //WordBool(__cdecl* ElementMatches)(Cardinal, PWChar, PWChar, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ElementMatches(uint element, string path, string value, out bool res);

        //WordBool(__cdecl* HasArrayItem)(Cardinal, PWChar, PWChar, PWChar, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool HasArrayItem(uint element, string path, string subpath, string value,
            out bool res);

        //WordBool(__cdecl* GetArrayItem)(Cardinal, PWChar, PWChar, PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetArrayItem(uint element, string path, string subpath, string value,
            out uint res);

        //WordBool(__cdecl* AddArrayItem)(Cardinal, PWChar, PWChar, PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool AddArrayItem(uint element, string path, string subpath, string value,
            out uint res);

        //WordBool(__cdecl* RemoveArrayItem)(Cardinal, PWChar, PWChar, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RemoveArrayItem(uint element, string path, string subpath, string value);

        //WordBool(__cdecl* MoveArrayItem)(Cardinal, Integer);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool MoveArrayItem(uint element, int value);

        //WordBool(__cdecl* CopyElement)(Cardinal, Cardinal, WordBool, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CopyElement(uint elementOne, uint elementTwo, bool asNew, out uint res);

        //WordBool(__cdecl* FindNextElement)(Cardinal, PWChar, WordBool, WordBool, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FindNextElement(uint element, string search, bool byPath, bool byValue,
            out uint res);

        //WordBool(__cdecl* FindPreviousElement)(Cardinal, PWChar, WordBool, WordBool, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FindPreviousElement(uint element, string search, bool byPath, bool byValue,
            out uint res);

        //WordBool(__cdecl* GetSignatureAllowed)(Cardinal, PWChar, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetSignatureAllowed(uint element, string signature, out bool res);

        //WordBool(__cdecl* GetAllowedSignatures)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetAllowedSignatures(uint element, out int len);

        //WordBool(__cdecl* GetIsModified)(Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetIsModified(uint element, out bool res);

        //WordBool(__cdecl* GetIsEditable)(Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetIsEditable(uint element, out bool res);

        // ORIGINAL ERROR: Missing
        //WordBool(__cdecl* SetIsEditable)(Cardinal, WordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetIsEditable(uint element, bool isEditable);

        //WordBool(__cdecl* GetIsRemoveable)(Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetIsRemoveable(uint element, out bool res);

        //WordBool(__cdecl* GetCanAdd)(Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetCanAdd(uint element, out bool res);

        //WordBool(__cdecl* SortKey)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SortKey(uint element, out int len);

        //WordBool(__cdecl* ElementType)(Cardinal, PByte);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ElementType(uint element, out Byte res);

        //WordBool(__cdecl* DefType)(Cardinal, PByte);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DefType(uint element, out Byte res);

        //WordBool(__cdecl* SmashType)(Cardinal, PByte);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SmashType(uint element, out Byte res);

        //WordBool(__cdecl* ValueType)(Cardinal, PByte);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ValueType(uint element, out Byte res);

        //WordBool(__cdecl* IsSorted)(Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsSorted(uint element, out bool res);

        // PLUGIN ERROR METHODS
        //WordBool(__cdecl* CheckForErrors)(Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool CheckForErrors(uint handle);

        //WordBool(__cdecl* GetErrorThreadDone)();
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetErrorThreadDone();

        //WordBool(__cdecl* GetErrors)(PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetErrors(out int len);

        //WordBool(__cdecl* RemoveIdenticalRecords)(Cardinal, WordBool, WordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool RemoveIdenticalRecords(uint handle, bool removeItms, bool removeItpos);

        // SERIALIZATION METHODS
        //WordBool(__cdecl* ElementToJson)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ElementToJson(uint handle, out int len);

        //WordBool(__cdecl* ElementFromJson)(Cardinal, PWChar, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ElementFromJson(uint handle, string path, string json);

        // ELEMENT VALUE METHODS
        //WordBool(__cdecl* Name)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Name(uint element, out int len);

        //WordBool(__cdecl* LongName)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool LongName(uint element, out int len);

        //WordBool(__cdecl* DisplayName)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool DisplayName(uint element, out int len);

        //WordBool(__cdecl* Path)(Cardinal, WordBool, WordBool, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Path(uint element, bool _short, bool local, out int len);

        //WordBool(__cdecl* Signature)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Signature(uint element, out int len);

        //WordBool(__cdecl* GetValue)(Cardinal, PWChar, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetValue(uint element, string path, out int len);

        //WordBool(__cdecl* SetValue)(Cardinal, PWChar, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetValue(uint element, string path, string value);

        //WordBool(__cdecl* GetIntValue)(Cardinal, PWChar, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetIntValue(uint element, string path, out int res);

        //WordBool(__cdecl* SetIntValue)(Cardinal, PWChar, Integer);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetIntValue(uint element, string path, int value);

        //WordBool(__cdecl* GetUIntValue)(Cardinal, PWChar, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetUIntValue(uint element, string path, out uint res);

        //WordBool(__cdecl* SetUIntValue)(Cardinal, PWChar, Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetUIntValue(uint element, string path, uint value);

        //WordBool(__cdecl* GetFloatValue)(Cardinal, PWChar, PDouble);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetFloatValue(uint element, string path, out Double res);

        //WordBool(__cdecl* SetFloatValue)(Cardinal, PWChar, double);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetFloatValue(uint element, string path, Double value);

        //WordBool(__cdecl* GetFlag)(Cardinal, PWChar, PWChar, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetFlag(uint element, string path, string flagName, out bool res);

        //WordBool(__cdecl* SetFlag)(Cardinal, PWChar, PWChar, WordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetFlag(uint element, string path, string flagName, bool value);

        //WordBool(__cdecl* GetEnabledFlags)(Cardinal, PWChar, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetEnabledFlags(uint element, string path, out int len);

        //WordBool(__cdecl* SetEnabledFlags)(Cardinal, PWChar, PWChar);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetEnabledFlags(uint element, string path, string value);

        //WordBool(__cdecl* GetAllFlags)(Cardinal, PWChar, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetAllFlags(uint element, string path, out int len);

        //WordBool(__cdecl* GetEnumOptions)(Cardinal, PWChar, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetEnumOptions(uint element, string path, out int len);

        //WordBool(__cdecl* SignatureFromName)(PWChar, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SignatureFromName(string name, out int len);

        //WordBool(__cdecl* NameFromSignature)(PWChar, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool NameFromSignature(string signature, out int len);

        //WordBool(__cdecl* GetSignatureNameMap)(PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetSignatureNameMap(out int len);

        // RECORD HANDLING METHODS
        //WordBool(__cdecl* GetFormID)(Cardinal, PCardinal, WordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetFormID(uint element, out uint res, bool native);

        //WordBool(__cdecl* SetFormID)(Cardinal, Cardinal, WordBool, WordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetFormID(uint element, uint formId, bool native, bool fixouts);

        //WordBool(__cdecl* GetRecord)(Cardinal, Cardinal, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetRecord(uint element, uint formId, bool searchMasters, out uint res);

        //WordBool(__cdecl* GetRecords)(Cardinal, PWChar, WordBool, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetRecords(uint element, string search, bool includeOverrides, out int len);

        //WordBool(__cdecl* GetOverrides)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetOverrides(uint element, out int len);

        //WordBool(__cdecl* GetReferencedBy)(Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetReferencedBy(uint element, out int len);

        //WordBool(__cdecl* GetMasterRecord)(Cardinal, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetMasterRecord(uint element, out uint res);

        //WordBool(__cdecl* GetPreviousOverride)(Cardinal, Cardinal, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetPreviousOverride(uint elementOne, uint elementTwo, out uint res);

        //WordBool(__cdecl* GetWinningOverride)(Cardinal, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetWinningOverride(uint element, out uint res);

        //WordBool(__cdecl* FindNextRecord)(Cardinal, PWChar, WordBool, WordBool, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FindNextRecord(uint element, string search, bool byEdId, bool byName,
            out uint res);

        //WordBool(__cdecl* FindPreviousRecord)(Cardinal, PWChar, WordBool, WordBool, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FindPreviousRecord(uint element, string search, bool byEdId, bool byName,
            out uint res);

        //WordBool(__cdecl* FindValidReferences)(Cardinal, PWChar, PWChar, Integer, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FindValidReferences(uint element, string signature, string search, int limitTo,
            out int len);

        //WordBool(__cdecl* ExchangeReferences)(Cardinal, Cardinal, Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ExchangeReferences(uint element, uint oldFormId, uint newFormId);

        //WordBool(__cdecl* IsMaster)(Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsMaster(uint element, out bool res);

        //WordBool(__cdecl* IsInjected)(Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsInjected(uint element, out bool res);

        //WordBool(__cdecl* IsOverride)(Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsOverride(uint element, out bool res);

        //WordBool(__cdecl* IsWinningOverride)(Cardinal, PWordBool);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsWinningOverride(uint element, out bool res);

        //WordBool(__cdecl* GetNodes)(Cardinal, PCardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetNodes(uint element, out uint res);

        //WordBool(__cdecl* GetConflictData)(Cardinal, Cardinal, PByte, PByte);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetConflictData(uint elementOne, uint elementTwo, out Byte resOne,
            out Byte resTwo);

        //WordBool(__cdecl* GetNodeElements)(Cardinal, Cardinal, PInteger);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool GetNodeElements(uint nodes, uint element, out int res);

        // FILTERING METHODS
        //WordBool(__cdecl* FilterRecord)(Cardinal);
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool FilterRecord(uint handle);

        //WordBool(__cdecl* ResetFilter)();
        [DllImport(DllPath, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool ResetFilter();
    }
}