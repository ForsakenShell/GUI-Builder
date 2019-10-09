/*
 * Preset.cs
 *
 * NIFBuilder border presets
 *
 * User: 1000101
 * Date: 13/02/2019
 * Time: 5:15 PM
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;

public static partial class NIFBuilder
{
    
    public class Preset
    {
        public string       Name;
        
        public List<Preset> SubSets;
        
        public float        NodeLength;
        public float        SlopeAllowance;
        public float        GradientHeight;
        public float        GroundOffset;
        public float        GroundSink;
        public string       TargetSuffix;
        public string       MeshSubDirectory;
        public string       FilePrefix;
        public string       FileSuffix;
        public bool         CreateImportData;
        
        public bool         SetOfPresets
        { get{ return !SubSets.NullOrEmpty(); } }
        
        public Preset( string name, float nodeLength, float slopeAllowance, float gradientHeight, float groundOffset, float groundSink, string meshSubDirectory, string filePrefix, string targetSuffix = "", string fileSuffix = "", bool createImportData = false )
        {
            Name                = name;
            NodeLength          = nodeLength;
            SlopeAllowance      = slopeAllowance;
            GradientHeight      = gradientHeight;
            GroundOffset        = groundOffset;
            GroundSink          = groundSink;
            TargetSuffix        = targetSuffix;
            MeshSubDirectory    = meshSubDirectory;
            FilePrefix          = filePrefix;
            FileSuffix          = fileSuffix;
            CreateImportData    = createImportData;
        }
        
        public Preset( string name, float nodeLength, float slopeAllowance, List<Preset> subSets )
        {
            Name                = name;
            SubSets             = subSets;
            NodeLength          = nodeLength;
            SlopeAllowance      = slopeAllowance;
            CreateImportData    = subSets.Any( p => p.CreateImportData );
        }
        
        public static class Workshop
        {
            public static Preset        VanillaGroundSink   = new Preset( "Vanilla Workshop w/GS"   ,  128.000f,    0.0100f,  256.0000f,    0.0000f,   64.0000f,  "Workshop"            , "WorkshopBorder" );
            public static Preset        Vanilla             = new Preset( "Vanilla Workshop"        ,  128.000f,    0.0100f,  256.0000f,    0.0000f,    0.0000f,  "Workshop"            , "WorkshopBorder" );
            
        }
        
        public static class SubDivision
        {
            public static Preset        Tall                = new Preset( "Tall"                    ,  128.000f,    0.0100f, 1024.0000f, 1024.0000f,   64.0000f, @"ESM\ATC\SubDivisions", "ESM_ATC_", "Tall Height"      , "Border", true );
            public static Preset        Medium              = new Preset( "Medium"                  ,  128.000f,    0.0100f,  512.0000f,  512.0000f,   64.0000f, @"ESM\ATC\SubDivisions", "ESM_ATC_", "Medium Height"    , "Border" );
            public static Preset        Vanilla             = new Preset( "Vanilla Workshop"        ,  128.000f,    0.0100f,  256.0000f,    0.0000f,   64.0000f, @"ESM\ATC\SubDivisions", "ESM_ATC_", "Vanilla Height"   , "Border" );
            public static Preset        None                = new Preset( "None"                    ,  128.000f,    0.0100f,    0.0000f,    0.0000f,    0.0000f, @"ESM\ATC\SubDivisions", "ESM_ATC_", "No Border"        , "Border" );
            public static Preset        FullSet             = new Preset( "Full ATC Set"            ,  128.000f,    0.0100f, new List<Preset>{ SubDivision.Tall, SubDivision.Medium, SubDivision.Vanilla, SubDivision.None } );
        }
        
        public static List<Preset>      WorkshopPresets     = new List<Preset>{ Workshop.VanillaGroundSink, Workshop.Vanilla };
        public static List<Preset>      SubDivisionPresets  = new List<Preset>{ SubDivision.FullSet, SubDivision.Tall, SubDivision.Medium, SubDivision.Vanilla, SubDivision.None };
        
    }
    
}
