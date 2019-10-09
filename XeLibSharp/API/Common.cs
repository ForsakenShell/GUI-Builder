using System;
using System.Globalization;

namespace XeLib.API
{
    public static class Common // To what???  What forms or elements should these be used on?
    {
        /*
        public static bool HasKeyword( Handle handle, string value)
        {
            return Elements.HasArrayItem(handle, "Keywords", "", value);
        }

        public static void AddKeyword( Handle handle, string value)
        {
            Elements.AddArrayItem(handle, "Keywords", "", value);
        }

        public static void RemoveKeyword( Handle handle, string value)
        {
            Elements.RemoveArrayItem(handle, "Keywords", "", value);
        }

        public static bool HasFormId( Handle handle, string value)
        {
            return Elements.HasArrayItem(handle, "FormIDs", "", value);
        }

        public static void AddFormId( Handle handle, string value)
        {
            Elements.AddArrayItem(handle, "FormIDs", "", value);
        }

        public static void RemoveFormId( Handle handle, string value)
        {
            Elements.RemoveArrayItem(handle, "FormIDs", "", value);
        }

        public static bool HasMusicTrack( Handle handle, string value)
        {
            return Elements.HasArrayItem(handle, "Music Tracks", "", value);
        }

        public static void AddMusicTrack( Handle handle, string value)
        {
            Elements.AddArrayItem(handle, "Music Tracks", "", value);
        }

        public static void RemoveMusicTrack( Handle handle, string value)
        {
            Elements.RemoveArrayItem(handle, "Music Tracks", "", value);
        }

        public static bool HasFootstep( Handle handle, string value)
        {
            return Elements.HasArrayItem(handle, "Footstep Sets", "", value);
        }

        public static void AddFootstep( Handle handle, string value)
        {
            Elements.AddArrayItem(handle, "Footstep Sets", "", value);
        }

        public static void RemoveFootstep( Handle handle, string value)
        {
            Elements.RemoveArrayItem(handle, "Footstep Sets", "", value);
        }

        public static bool HasAdditionalRace( Handle handle, string value)
        {
            return Elements.HasArrayItem(handle, "Additional Races", "", value);
        }

        public static void AddAdditionalRace( Handle handle, string value)
        {
            Elements.AddArrayItem(handle, "Additional Races", "", value);
        }

        public static void RemoveAdditionalRace( Handle handle, string value)
        {
            Elements.RemoveArrayItem(handle, "Additional Races", "", value);
        }

        public static bool HasEffect( Handle handle, string value)
        {
            return Elements.HasArrayItem(handle, "Effects", "EFID - Base Effect", value);
        }

        public static Handle GetEffect( Handle handle, string value)
        {
            return Elements.GetArrayItem(handle, "Effects", "EFID - Base Effect", value);
        }

        public static void AddEffect( Handle handle, string value, string magnitude, string area, string duration)
        {
            var newItem = Elements.AddArrayItem(handle, "Effects", "EFID - Base Effect", value);
            ElementValues.SetValue(newItem, @"EFIT\Magnitude", magnitude);
            ElementValues.SetValue(newItem, @"EFIT\Area", area);
            ElementValues.SetValue(newItem, @"EFIT\Duration", duration);
        }

        public static void RemoveEffect( Handle handle, string value)
        {
            Elements.RemoveArrayItem(handle, "Effects", "EFID - Base Effect", value);
        }

        public static bool HasItem( Handle handle, string value)
        {
            return Elements.HasArrayItem(handle, "Items", @"CNTO\Item", value);
        }

        public static Handle GetItem( Handle handle, string value)
        {
            return Elements.GetArrayItem(handle, "Items", @"CNTO\Item", value);
        }

        public static void AddItem( Handle handle, string value, string count)
        {
            var newItem = Elements.AddArrayItem(handle, "Items", @"CNTO\Item", value);
            ElementValues.SetValue(newItem, @"CNTO\Count", count);
        }

        public static void RemoveItem( Handle handle, string value)
        {
            Elements.RemoveArrayItem(handle, "Items", @"CNTO\Item", value);
        }

        public static bool HasLeveledEntry( Handle handle, string value)
        {
            return Elements.HasArrayItem(handle, "Leveled List Entries", @"LVLO\Reference", value);
        }

        public static Handle GetLeveledEntry( Handle handle, string value)
        {
            return Elements.GetArrayItem(handle, "Leveled List Entries", @"LVLO\Reference", value);
        }

        public static void AddLeveledEntry( Handle handle, string value, string level, string count)
        {
            var newItem = Elements.AddArrayItem(handle, "Leveled List Entries", @"LVLO\Reference", value);
            ElementValues.SetValue(newItem, @"LVLO\Level", level);
            ElementValues.SetValue(newItem, @"LVLO\Count", count);
        }

        public static void RemoveLeveledEntry( Handle handle, string value)
        {
            Elements.RemoveArrayItem(handle, "Leveled List Entries", @"LVLO\Reference", value);
        }
        
        */
        
        public static bool HasScript( ElementHandle handle, string value )
        {
            return Elements.HasArrayItemEx( handle.XHandle, @"VMAD\Scripts", "scriptName", value );
        }

        public static ElementHandle GetScript( ElementHandle handle, string value )
        {
            return Elements.GetArrayItemEx<ElementHandle>( handle.XHandle, @"VMAD\Scripts", "scriptName", value );
        }

        public static void AddScript( ElementHandle handle, string value, string name )
        {
            var newItem = Elements.AddArrayItemEx<ElementHandle>( handle.XHandle, @"VMAD\Scripts", "scriptName", value );
            ElementValues.SetValueEx( newItem.XHandle, "Flags", name );
        }

        public static void RemoveScript( ElementHandle handle, string value )
        {
            Elements.RemoveArrayItemEx( handle.XHandle, @"VMAD\Scripts", "scriptName", value );
        }

        public static bool HasScriptProperty( ElementHandle handle, string value )
        {
            return Elements.HasArrayItemEx( handle.XHandle, "Properties", "propertyName", value );
        }

        public static ElementHandle GetScriptProperty( ElementHandle handle, string value )
        {
            return Elements.GetArrayItemEx<ElementHandle>( handle.XHandle, "Properties", "propertyName", value );
        }

        public static void AddScriptProperty( ElementHandle handle, string value, string type, string flags )
        {
            var newItem = Elements.AddArrayItemEx<ElementHandle>( handle.XHandle, "Properties", "propertyName", value );
            ElementValues.SetValueEx( newItem.XHandle, "Type", type );
            ElementValues.SetValueEx( newItem.XHandle, "Flags", flags );
        }

        public static void RemoveScriptProperty( ElementHandle handle, string value )
        {
            Elements.RemoveArrayItemEx( handle.XHandle, "Properties", "propertyName", value );
        }
        
        /*
        public static bool HasCondition( Handle handle, string value)
        {
            return Elements.HasArrayItem(handle, "Conditions", @"CTDA\Function", value);
        }

        public static Handle GetCondition( Handle handle, string value)
        {
            return Elements.GetArrayItem(handle, "Conditions", @"CTDA\Function", value);
        }

        public static void AddCondition( Handle handle, string value, string type, string comparisonValue,
            string parameterOne)
        {
            var newItem = Elements.AddArrayItem(handle, "Conditions", @"CTDA\Function", value);
            ElementValues.SetValue(newItem, @"CTDA\Type", type);
            ElementValues.SetValue(newItem, @"CTDA\Comparison Value", comparisonValue);
            ElementValues.SetValue(newItem, @"CTDA\Parameter #1", parameterOne);
        }

        public static void RemoveCondition( Handle handle, string value)
        {
            Elements.RemoveArrayItem(handle, "Conditions", @"CTDA\Function", value);
        }

        public static Double GetGoldValue( Handle handle)
        {
            return Double.Parse(ElementValues.GetValue(handle, @"DATA\Value"));
        }

        public static void SetGoldValue( Handle handle, Double value)
        {
            ElementValues.SetValue(handle, @"DATA\Value", value.ToString(CultureInfo.InvariantCulture));
        }

        public static Double GetWeight( Handle handle)
        {
            return Double.Parse(ElementValues.GetValue(handle, @"DATA\Weight"));
        }

        public static void SetWeight( Handle handle, Double value)
        {
            ElementValues.SetValue(handle, @"DATA\Weight", value.ToString(CultureInfo.InvariantCulture));
        }

        public static Double GetDamage( Handle handle)
        {
            return Double.Parse(ElementValues.GetValue(handle, @"DATA\Damage"));
        }

        public static void SetDamage( Handle handle, Double value)
        {
            ElementValues.SetValue(handle, @"DATA\Damage", value.ToString(CultureInfo.InvariantCulture));
        }

        public static Double GetArmorRating( Handle handle)
        {
            return Double.Parse(ElementValues.GetValue(handle, "DNAM"));
        }

        public static void SetArmorRating( Handle handle, Double value)
        {
            ElementValues.SetValue(handle, "DNAM", value.ToString(CultureInfo.InvariantCulture));
        }

        public static bool GetIsFemale( Handle handle)
        {
            return ElementValues.GetFlag(handle, @"ACBS\Flags", "Female");
        }

        public static void SetIsFemale( Handle handle, bool state)
        {
            ElementValues.SetFlag(handle, @"ACBS\Flags", "Female", state);
        }

        public static bool GetIsEssential( Handle handle)
        {
            return ElementValues.GetFlag(handle, @"ACBS\Flags", "Essential");
        }

        public static void SetIsEssential( Handle handle, bool state)
        {
            ElementValues.SetFlag(handle, @"ACBS\Flags", "Essential", state);
        }

        public static bool GetIsUnique( Handle handle)
        {
            return ElementValues.GetFlag(handle, @"ACBS\Flags", "Unique");
        }

        public static void SetIsUnique( Handle handle, bool state)
        {
            ElementValues.SetFlag(handle, @"ACBS\Flags", "Unique", state);
        }
        */
    }
}