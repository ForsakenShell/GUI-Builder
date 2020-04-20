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
        
        public delegate void    Serializer( Preset preset );

        public string           Name;

        public Serializer       onSerialize = null;

        public bool             AllowWriteback      { get { return onSerialize != null; } }

        public List<Preset>     SubSets;
        
        public float            NodeLength;
        public double           AngleAllowance;
        public double           SlopeAllowance;
        public float            GradientHeight;
        public float            GroundOffset;
        public float            GroundSink;
        public string           TargetSubDirectory;
        public string           MeshSubDirectory;
        public bool             HighPrecisionFloats;
        public bool             CreateImportData;
        public bool             UseExistingSTATEditorIDs;
        public bool             UseExistingNIFFilePaths;
        
        public bool             SetOfPresets
        { get{ return !SubSets.NullOrEmpty(); } }

        public void             Serialize()
        {
            onSerialize?.Invoke( this );
        }

        public Preset( string name )
        {
            Name = name;
        }

        public Preset( string name, bool highPrecisionFloats, float nodeLength, double angleAllowance, double slopeAllowance, float gradientHeight, float groundOffset, float groundSink, string meshSubDirectory, bool createImportData = false, string targetSubDirectory = null, bool useExistingSTATEditorIDs = false, bool useExistingNIFFilePaths = false )
        {
            Name                        = name;
            HighPrecisionFloats         = highPrecisionFloats;
            NodeLength                  = nodeLength;
            AngleAllowance              = angleAllowance;
            SlopeAllowance              = slopeAllowance;
            GradientHeight              = gradientHeight;
            GroundOffset                = groundOffset;
            GroundSink                  = groundSink;
            TargetSubDirectory          = targetSubDirectory;
            MeshSubDirectory            = meshSubDirectory;
            CreateImportData            = createImportData;
            UseExistingSTATEditorIDs    = useExistingSTATEditorIDs;
            UseExistingNIFFilePaths     = useExistingNIFFilePaths;
        }
        
        public Preset( string name, bool highPrecisionFloats, float nodeLength, double angleAllowance, double slopeAllowance, List<Preset> subSets )
        {
            Name                        = name;
            HighPrecisionFloats         = highPrecisionFloats;
            SubSets                     = subSets;
            NodeLength                  = nodeLength;
            AngleAllowance              = angleAllowance;
            SlopeAllowance              = slopeAllowance;
            CreateImportData            = subSets.Any( p => p.CreateImportData );
        }

        public Preset( Preset p )
        {
            Name                        = p.Name;
            HighPrecisionFloats         = p.HighPrecisionFloats;
            NodeLength                  = p.NodeLength;
            AngleAllowance              = p.AngleAllowance;
            SlopeAllowance              = p.SlopeAllowance;
            GradientHeight              = p.GradientHeight;
            GroundOffset                = p.GroundOffset;
            GroundSink                  = p.GroundSink;
            TargetSubDirectory          = p.TargetSubDirectory;
            MeshSubDirectory            = p.MeshSubDirectory;
            CreateImportData            = p.CreateImportData;
            UseExistingSTATEditorIDs    = p.UseExistingSTATEditorIDs;
            UseExistingNIFFilePaths     = p.UseExistingNIFFilePaths;
        }

        public static class Workshop
        {
            public static Preset        Custom              = new Preset( "Custom" , GUIBuilder.BorderNode.DEFAULT_NIF_HIGH_PRECISION, GUIBuilder.BorderNode.DEFAULT_NODE_LENGTH, GUIBuilder.BorderNode.DEFAULT_ANGLE_ALLOWANCE, GUIBuilder.BorderNode.DEFAULT_SLOPE_ALLOWANCE, 256.0f, 0.0f, 64.0f, "Workshop", true, null, true, true );

            public static Preset        VanillaGroundSink   = new Preset( "NIFBuilder.Preset.VanillaWorkshopGS".Translate()  , false,  128.000f,    1.5000f,    0.0100f,  256.0000f,    0.0000f,   64.0000f,  "Workshop"                   , true, null, true, true );
            public static Preset        Vanilla             = new Preset( "NIFBuilder.Preset.VanillaWorkshop".Translate()    , false,  128.000f,    1.5000f,    0.0100f,  256.0000f,    0.0000f,    0.0000f,  "Workshop"                   , true, null, true, true );
            public static Preset        VanillaGroundSinkLP = new Preset( "NIFBuilder.Preset.VanillaWorkshopGSLP".Translate(), false,  128.000f,    5.0000f,    0.0500f,  256.0000f,    0.0000f,   64.0000f,  "Workshop"                   , true, null, true, true );
            public static Preset        VanillaLP           = new Preset( "NIFBuilder.Preset.VanillaWorkshopLP".Translate()  , false,  128.000f,    5.0000f,    0.0500f,  256.0000f,    0.0000f,    0.0000f,  "Workshop"                   , true, null, true, true );
            public static Preset        Floating            = new Preset( "NIFBuilder.Preset.Floating".Translate()           , false, 1000.000f,    5.0000f,    0.0500f,  500.0000f,  500.0000f,    0.0000f,  "Workshop"                   , true, null, true, true );

        }

        public static class SubDivision
        {
            public static Preset        Custom              = new Preset( "Custom"                                           , true ,    512.0f,    2.5000f,    0.0500f,  256.0000f,    0.0000f,   64.0000f, @"ESM\ATC\SubDivisions\{name}", true , "Custom"         );

            public static Preset        Tall                = new Preset( "NIFBuilder.Preset.ATCTall".Translate()            , true ,  512.000f,    2.5000f,    0.0500f, 1024.0000f, 1024.0000f,   64.0000f, @"ESM\ATC\SubDivisions\{name}", true , "Tall Height"    );
            public static Preset        Medium              = new Preset( "NIFBuilder.Preset.ATCMedium".Translate()          , true ,  512.000f,    2.5000f,    0.0500f,  512.0000f,  512.0000f,   64.0000f, @"ESM\ATC\SubDivisions\{name}", false, "Medium Height"  );
            public static Preset        Vanilla             = new Preset( "NIFBuilder.Preset.ATCSmall".Translate()           , true ,  512.000f,    2.5000f,    0.0500f,  256.0000f,    0.0000f,   64.0000f, @"ESM\ATC\SubDivisions\{name}", false, "Vanilla Height" );
            public static Preset        None                = new Preset( "NIFBuilder.Preset.ATCNone".Translate()            , true , 4096.000f,    2.5000f,    0.0500f,    0.0000f,    0.0000f,    0.0000f, @"ESM\ATC\SubDivisions\{name}", false, "No Border"      );
            public static Preset        FullSet             = new Preset( "NIFBuilder.Preset.ATCAll".Translate()             , true ,  512.000f,    2.5000f,    0.0500f, new List<Preset>{ SubDivision.Tall, SubDivision.Medium, SubDivision.Vanilla, SubDivision.None } );
        }
        
        public static List<Preset>      WorkshopPresets     = new List<Preset>{ Workshop.VanillaGroundSink, Workshop.Vanilla, Workshop.VanillaGroundSinkLP, Workshop.VanillaLP, Workshop.Floating };
        public static List<Preset>      SubDivisionPresets  = new List<Preset>{ SubDivision.FullSet, SubDivision.Tall, SubDivision.Medium, SubDivision.Vanilla, SubDivision.None };
        
        public override string          ToString()
        {
            return string.Format(
                "[ Name = \"{0}\" :: HighPrecisionFloats = {1} :: NodeLength = {2} :: SlopeAllowance = {3} :: GradientHeight = {4} :: GroundOffset = {5} :: GroundSink = {6} :: TargetSuffix = \"{7}\" :: MeshSubDirectory = \"{8}\" :: CreateImportData = {9} ]",
                Name,
                HighPrecisionFloats,
                NodeLength,
                SlopeAllowance,
                GradientHeight,
                GroundOffset,
                GroundSink,
                TargetSubDirectory,
                MeshSubDirectory,
                CreateImportData );
        }

    }
    
}
