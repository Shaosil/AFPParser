using AFPParser.ImageSelfDefiningFields;
using AFPParser.PTXControlSequences;
using AFPParser.StructuredFields;
using AFPParser.Triplets;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace AFPParser
{
    public static class Lookups
    {
        public enum DataTypes { EMPTY, BITS, CHAR, CODE, TRIPS, IMAGESDFS, UBIN, SBIN, COLOR };

        #region Print File Heirarchy
        // Associates a type (standalone or begin container tag) with one or more parent begin types that may contain it.
        public static IReadOnlyDictionary<Type, Type[]> FieldsParentOptions = new Dictionary<Type, Type[]>()
        {
            // Print File
            { typeof(BRG), new[] { typeof(BPF) } },
            { typeof(BDI), new[] { typeof(BPF) } },

            // Resource Group/Resource
            { typeof(BRS), new[] { typeof(BRG) } },
            { typeof(BMO), new[] { typeof(BRS) } },
            { typeof(BPS), new[] { typeof(BRS) } },
            { typeof(BFM), new[] { typeof(BRS) } },
            { typeof(BCF), new[] { typeof(BRS) } },
            { typeof(BCP), new[] { typeof(BRS) } },
            { typeof(BFN), new[] { typeof(BRS) } },
            
            // Document index
            { typeof(IEL), new[] { typeof(BDI) } },

            // Resource environment group
            { typeof(PPO), new[] { typeof(BSG) } },

            // Page
            { typeof(IPO), new[] { typeof(BPG) } },

            // Barcode, graphics, image, and presentation text objects
            { typeof(BDA), new[] { typeof(BBC) } },
            { typeof(GAD), new[] { typeof(BGR) } },
            { typeof(IPD), new[] { typeof(BIM) } },
            { typeof(PTX), new[] { typeof(BPT) } },
            { typeof(OCD), new[] { typeof(BOC) } },
            { typeof(IOC), new[] { typeof(BII) } },
            { typeof(IID), new[] { typeof(BII) } },
            { typeof(ICP), new[] { typeof(BII) } },
            { typeof(IRD), new[] { typeof(BII) } },

            // Font objects
            { typeof(CFC), new[] { typeof(BCF) } },
            { typeof(CFI), new[] { typeof(BCF) } },
            { typeof(ECF), new[] { typeof(BCF) } },
            { typeof(CPD), new[] { typeof(BCP) } },
            { typeof(CPC), new[] { typeof(BCP) } },
            { typeof(CPI), new[] { typeof(BCP) } },
            { typeof(ECP), new[] { typeof(BCP) } },
            { typeof(FND), new[] { typeof(BFN) } },
            { typeof(FNC), new[] { typeof(BFN) } },
            { typeof(FNM), new[] { typeof(BFN) } },
            { typeof(FNO), new[] { typeof(BFN) } },
            { typeof(FNP), new[] { typeof(BFN) } },
            { typeof(FNI), new[] { typeof(BFN) } },
            { typeof(FNN), new[] { typeof(BFN) } },
            { typeof(FNG), new[] { typeof(BFN) } },
            { typeof(EFN), new[] { typeof(BFN) } },

            // Active environment group
            { typeof(MPS), new[] { typeof(BAG) } },
            { typeof(PGD), new[] { typeof(BAG) } },
            { typeof(PTD1), new[] { typeof(BAG) } },
            { typeof(PTD2), new[] { typeof(BAG) } },
            { typeof(MCF1), new[] { typeof(BAG) } },
            { typeof(MCF2), new[] { typeof(BAG) } },

            // Object environment group
            { typeof(MBC), new[] { typeof(BOG) } },
            { typeof(BDD), new[] { typeof(BOG) } },
            { typeof(MGO), new[] { typeof(BOG) } },
            { typeof(GDD), new[] { typeof(BOG) } },
            { typeof(MIO), new[] { typeof(BOG) } },
            { typeof(IDD), new[] { typeof(BOG) } },
            { typeof(MCD), new[] { typeof(BOG) } },
            { typeof(CDD), new[] { typeof(BOG) } },

            // Form map
            { typeof(BDG), new[] { typeof(BFM) } },

            // Document environment group
            { typeof(PFC), new[] { typeof(BDG) } },
            { typeof(MSU), new[] { typeof(BDG) } },

            // Medium Map
            { typeof(MMT), new[] { typeof(BMM) } },
            { typeof(MCC), new[] { typeof(BMM) } },
            { typeof(MMC), new[] { typeof(BMM) } },
            { typeof(PMC), new[] { typeof(BMM) } },

            // Multiple parent options for containers
            { typeof(BDT), new[] { typeof(BPF), typeof(BRG) } }, // Document container
            { typeof(BBC), new[] { typeof(BRS), typeof(BPG), typeof(BMO), typeof(BPS) } }, // Barcode container
            { typeof(BGR), new[] { typeof(BRS), typeof(BPG), typeof(BMO), typeof(BPS) } }, // Graphics container
            { typeof(BII), new[] { typeof(BRS), typeof(BPG), typeof(BMO), typeof(BPS) } }, // IM Image Container
            { typeof(BIM), new[] { typeof(BRS), typeof(BPG), typeof(BMO), typeof(BPS) } }, // IOCA Image container
            { typeof(BOC), new[] { typeof(BRS), typeof(BPG), typeof(BMO) } }, // Object container
            { typeof(BMM), new[] { typeof(BDT), typeof(BNG), typeof(BFM) } }, // Medium map container
            { typeof(BSG), new[] { typeof(BDT), typeof(BNG) } }, // Resource Environment group container
            { typeof(BPG), new[] { typeof(BDT), typeof(BNG) } }, // Page container
            { typeof(BNG), new[] { typeof(BDT), typeof(BNG) } }, // Page group container
            { typeof(BAG), new[] { typeof(BPG), typeof(BMO) } }, // Active environment container
            { typeof(BPT), new[] { typeof(BPG), typeof(BMO) } }, // Presentation text container
            { typeof(BOG), new[] { typeof(BBC), typeof(BGR), typeof(BIM), typeof(BOC) } }, // Object environment group container

            // Multiple parent options for individual fields
            { typeof(IMM), new[] { typeof(BDT), typeof(BNG) } }, // Invoke medium map
            { typeof(LLE), new[] { typeof(BDT), typeof(BDI), typeof(BPG), typeof(BNG), typeof(BMO) } }, // Link logical element
            { typeof(TLE), new[] { typeof(BDI), typeof(BPG), typeof(BNG), typeof(BMO) } }, // Tag logical element
            { typeof(MDR), new[] { typeof(BSG), typeof(BAG), typeof(BOG), typeof(BDG), typeof(BMM) } }, // Map data resource
            { typeof(MPO), new[] { typeof(BSG), typeof(BAG), typeof(BMM) } }, // Map page overlay
            { typeof(IOB), new[] { typeof(BPG), typeof(BMO) } }, // Include object
            { typeof(IPS), new[] { typeof(BPG), typeof(BMO) } }, // Include page segment
            { typeof(PEC), new[] { typeof(BAG), typeof(BOG), typeof(BDG), typeof(BMM) } }, // Presentation environment control
            { typeof(OBD), new[] { typeof(BAG), typeof(BOG) } }, // Object area descriptor
            { typeof(OBP), new[] { typeof(BAG), typeof(BOG) } }, // Object area position
            { typeof(MMO), new[] { typeof(BDG), typeof(BMM) } }, // Map medium overlay
            { typeof(PGP1), new[] { typeof(BDG), typeof(BMM) } }, // Page position format 1
            { typeof(PGP2), new[] { typeof(BDG), typeof(BMM) } }, // Page position format 2
            { typeof(MDD), new[] { typeof(BDG), typeof(BMM) } }, // Medium descriptor
            { typeof(MFC), new[] { typeof(BDG), typeof(BMM) } } // Medium finishing control
        };
        #endregion

        #region Structured Fields
        public static IReadOnlyDictionary<string, Type> StructuredFields = new Dictionary<string, Type>()
        {
            { "D3A8C9", typeof(BAG) }, // Begin Active Environment Group
            { "D3A8EB", typeof(BBC) }, // Begin Bar Code Object
            { "D3EEEB", typeof(BDA) }, // Bar Code Data
            { "D3A6EB", typeof(BDD) }, // Bar Code Data Descriptor
            { "D3A8C4", typeof(BDG) }, // Begin Document Environment Group
            { "D3A8A7", typeof(BDI) }, // Begin Document Index
            { "D3A8CA", typeof(BDM) }, // Begin Data Map
            { "D3A8A8", typeof(BDT) }, // Begin Document
            { "D3A8E3", typeof(BDX) }, // Begin Data Map Transmission Subcase
            { "D3A8C5", typeof(BFG) }, // Begin Form Environment Group (obsolete)
            { "D3A8CD", typeof(BFM) }, // Begin Form Map
            { "D3A8BB", typeof(BGR) }, // Begin Graphics Object
            { "D3A87B", typeof(BII) }, // Begin Image Object IM
            { "D3A8FB", typeof(BIM) }, // Begin Image Object IO
            { "D3A8CC", typeof(BMM) }, // Begin Medium Map
            { "D3A8DF", typeof(BMO) }, // Begin Overlay
            { "D3A8AD", typeof(BNG) }, // Begin Named Page Group
            { "D3A892", typeof(BOC) }, // Begin Object Container
            { "D3A8C7", typeof(BOG) }, // Begin Object Environment Group
            { "D3A8AF", typeof(BPG) }, // Begin Page
            { "D3A8CB", typeof(BPM) }, // Begin Page Map
            { "D3A85F", typeof(BPS) }, // Begin Page Segment
            { "D3A89B", typeof(BPT) }, // Begin Presentation Text Object
            { "D3A8A5", typeof(BPF) }, // Begin Print File
            { "D3A8CE", typeof(BRS) }, // Begin Resource
            { "D3A8C6", typeof(BRG) }, // Begin Resource Group
            { "D3A8D9", typeof(BSG) }, // Begin Resource Environment Group
            { "D3A7CA", typeof(CCP) }, // Conditional Processing Control
            { "D3A692", typeof(CDD) }, // Container Data Descriptor
            { "D3A79B", typeof(CTC) }, // Composed Text Control (obsolete)
            { "D3A6E3", typeof(DXD) }, // Data Map Transmission Subcase Descriptor
            { "D3A9C9", typeof(EAG) }, // End Active Environment Group
            { "D3A9EB", typeof(EBC) }, // End Bar Code Object
            { "D3A9C4", typeof(EDG) }, // End Document Environment Group
            { "D3A9A7", typeof(EDI) }, // End Document Index
            { "D3A9CA", typeof(EDM) }, // End Data Map
            { "D3A9A8", typeof(EDT) }, // End Document
            { "D3A9E3", typeof(EDX) }, // End Data Map Transmission Subcase
            { "D3A9C5", typeof(EFG) }, // End Form Environment Group (obsolete)
            { "D3A9CD", typeof(EFM) }, // End Form Map
            { "D3A9BB", typeof(EGR) }, // End Graphics Object
            { "D3A97B", typeof(EII) }, // End Image Object IM
            { "D3A9FB", typeof(EIM) }, // End Image Object IO
            { "D3A9CC", typeof(EMM) }, // End Medium Map
            { "D3A9DF", typeof(EMO) }, // End Overlay
            { "D3A9AD", typeof(ENG) }, // End Named Page Group
            { "D3A992", typeof(EOC) }, // End Object Container
            { "D3A9C7", typeof(EOG) }, // End Object Environment Group
            { "D3A9AF", typeof(EPG) }, // End Page
            { "D3A9CB", typeof(EPM) }, // End Page Map
            { "D3A95F", typeof(EPS) }, // End Page Segment
            { "D3A99B", typeof(EPT) }, // End Presentation Text
            { "D3A9A5", typeof(EPF) }, // End Print File
            { "D3A9CE", typeof(ERS) }, // End Resource
            { "D3A9C6", typeof(ERG) }, // End Resource Group
            { "D3A9D9", typeof(ESG) }, // End Resource Environment Group
            { "D3AAEC", typeof(FDS) }, // Fixed Data Size
            { "D3EEEC", typeof(FDX) }, // Fixed Data Text
            { "D3A6C5", typeof(FGD) }, // Form Environment Group Descriptor (obsolete)
            { "D3EEBB", typeof(GAD) }, // Graphics Data
            { "D3A6BB", typeof(GDD) }, // Graphics Data Descriptor
            { "D3AC7B", typeof(ICP) }, // Image Cell Position
            { "D3A6FB", typeof(IDD) }, // Image Data Descriptor IO
            { "D3ABCA", typeof(IDM) }, // Invoke Data Map
            { "D3B2A7", typeof(IEL) }, // Index Element
            { "D3A67B", typeof(IID) }, // Image Input Descriptor IM
            { "D3ABCC", typeof(IMM) }, // Invoke Medium Map
            { "D3AFC3", typeof(IOB) }, // Include Object
            { "D3A77B", typeof(IOC) }, // Image Output Control IM
            { "D3EEFB", typeof(IPD) }, // Image Picture Data IO
            { "D3AFAF", typeof(IPG) }, // Include Page
            { "D3AFD8", typeof(IPO) }, // Include Page Overlay
            { "D3AF5F", typeof(IPS) }, // Include Page Segment
            { "D3EE7B", typeof(IRD) }, // Image Raster Data IM
            { "D3B490", typeof(LLE) }, // Link Logical Element
            { "D3AAE7", typeof(LNC) }, // Line Descriptor Count
            { "D3A6E7", typeof(LND) }, // Line Descriptor
            { "D3ABEB", typeof(MBC) }, // Map Bar Code
            { "D3A288", typeof(MCC) }, // Medium Copy Count
            { "D3AB92", typeof(MCD) }, // Map Container Data
            { "D3B18A", typeof(MCF1) }, // Map Coded Font (Format 1)
            { "D3AB8A", typeof(MCF2) }, // Map Coded Font (Format 2)
            { "D3A688", typeof(MDD) }, // Medium Descriptor
            { "D3ABC3", typeof(MDR) }, // Map Data Resource
            { "D3A088", typeof(MFC) }, // Medium Finishing Control
            { "D3ABBB", typeof(MGO) }, // Map Graphic Object
            { "D3ABFB", typeof(MIO) }, // Map IO Image Object
            { "D3A788", typeof(MMC) }, // Medium Modification Control
            { "D3B1DF", typeof(MMO) }, // Map Medium Overlay
            { "D3AB88", typeof(MMT) }, // Map Media Type
            { "D3ABAF", typeof(MPG) }, // Map Page
            { "D3ABD8", typeof(MPO) }, // Map Page Overlay
            { "D3B15F", typeof(MPS) }, // Map Page Segment
            { "D3ABEA", typeof(MSU) }, // Map Suppression
            { "D3EEEE", typeof(StructuredFields.NOP) }, // No Operation
            { "D3A66B", typeof(OBD) }, // Object Area Descriptor
            { "D3AC6B", typeof(OBP) }, // Object Area Position
            { "D3EE92", typeof(OCD) }, // Object Container Data
            { "D3A7A8", typeof(PEC) }, // Presentation Environment Control
            { "D3B288", typeof(PFC) }, // Presentation Fidelity Control
            { "D3A6AF", typeof(PGD) }, // Page Descriptor
            { "D3ACAF", typeof(PGP1) }, // Page Position (Format 1)
            { "D3B1AF", typeof(PGP2) }, // Page Position (Format 2)
            { "D3A7AF", typeof(PMC) }, // Page Modification Control
            { "D3ADC3", typeof(PPO) }, // Preprocess Presentation Object
            { "D3A69B", typeof(PTD1) }, // Presentation Text Descriptor (Format 1)
            { "D3B19B", typeof(PTD2) }, // Presentation Text Descriptor (Format 2)
            { "D3EE9B", typeof(PTX) }, // Presentation Text Data
            { "D3A68D", typeof(RCD) }, // Record Descriptor
            { "D3A090", typeof(TLE) }, // Tag Logical Element
            { "D3A68E", typeof(XMD) }, // XML Descriptor
            { "D3A88A", typeof(BCF) }, // Begin Coded Font
            { "D3A887", typeof(BCP) }, // Begin Code Page
            { "D3A889", typeof(BFN) }, // Begin Font
            { "D3A78A", typeof(CFC) }, // Coded Font Control
            { "D38C8A", typeof(CFI) }, // Coded Font Index
            { "D3A787", typeof(CPC) }, // Code Page Control
            { "D3A687", typeof(CPD) }, // Code Page Descriptor
            { "D38C87", typeof(CPI) }, // Code Page Index
            { "D3A98A", typeof(ECF) }, // End Coded Font
            { "D3A987", typeof(ECP) }, // End Code Page
            { "D3A989", typeof(EFN) }, // End Font
            { "D3A789", typeof(FNC) }, // Font Control
            { "D3A689", typeof(FND) }, // Font Descriptor
            { "D3EE89", typeof(FNG) }, // Font Patterns
            { "D38C89", typeof(FNI) }, // Font Index
            { "D3A289", typeof(FNM) }, // Font Patterns Map
            { "D3AB89", typeof(FNN) }, // Font Names (Outline Fonts Only)
            { "D3AE89", typeof(FNO) }, // Font Orientation
            { "D3AC89", typeof(FNP) }  // Font Position
        };
        public static byte[] StructuredFieldID<T>()
        {
            // Return the byte array ID of the structured field type (or NOP if not found)
            string hexCode = StructuredFields.FirstOrDefault(f => f.Value == typeof(T)).Key;
            if (string.IsNullOrEmpty(hexCode))
                hexCode = StructuredFields.First(f => f.Value == typeof(StructuredFields.NOP)).Key;

            return Extensions.GetByteArrayFromHexString(hexCode);
        }
        #endregion

        #region Triplets
        public static IReadOnlyDictionary<byte, Type> Triplets = new Dictionary<byte, Type>()
        {
            { 0x01, typeof(GCSGID_CPGID_CCSID) },
            { 0x02, typeof(FullyQualifiedName) },
            { 0x04, typeof(MappingOption) },
            { 0x10, typeof(ObjectClassification) },
            { 0x18, typeof(MODCAInterchangeSet) },
            { 0x1F, typeof(FontDescriptorSpecification) },
            { 0x20, typeof(CodedGraphicCharacterSetGlobalIdentifier) },
            { 0x21, typeof(ObjectFunctionSetSpecification) },
            { 0x22, typeof(ExtendedResourceLocalIdentifier) },
            { 0x24, typeof(ResourceLocalIdentifier) },
            { 0x25, typeof(ResourceSectionNumber) },
            { 0x26, typeof(CharacterRotation) },
            { 0x2D, typeof(ObjectByteOffset) },
            { 0x36, typeof(AttributeValue) },
            { 0x43, typeof(DescriptorPosition) },
            { 0x45, typeof(MediaEjectControl) },
            { 0x46, typeof(PageOverlayConditionalProcessing) },
            { 0x47, typeof(ResourceUsageAttribute) },
            { 0x4B, typeof(MeasurementUnits) },
            { 0x4C, typeof(ObjectAreaSize) },
            { 0x4D, typeof(AreaDefinition) },
            { 0x4E, typeof(ColorSpecification) },
            { 0x50, typeof(EncodingSchemeID) },
            { 0x56, typeof(MediumMapPageNumber) },
            { 0x57, typeof(ObjectByteExtent) },
            { 0x58, typeof(ObjectStructuredFieldOffset) },
            { 0x59, typeof(ObjectStructuredFieldExtent) },
            { 0x5A, typeof(ObjectOffset) },
            { 0x5D, typeof(FontHorizontalScaleFactor) },
            { 0x5E, typeof(ObjectCount) },
            { 0x62, typeof(LocalDateAndTimeStamp) },
            { 0x63, typeof(CRCResourceManagement) },
            { 0x64, typeof(ObjectOriginIdentifier) },
            { 0x65, typeof(Comment) },
            { 0x68, typeof(MediumOrientation) },
            { 0x6D, typeof(ExtensionFont) },
            { 0x6C, typeof(ResourceObjectInclude) },
            { 0x70, typeof(PresentationSpaceResetMixing) },
            { 0x71, typeof(PresentationSpaceMixingRule) },
            { 0x72, typeof(UniversalDateAndTimeStamp) },
            { 0x74, typeof(TonerSaver) },
            { 0x75, typeof(ColorFidelity) },
            { 0x78, typeof(FontFidelity) },
            { 0x79, typeof(MetricAdjustment) },
            { 0x80, typeof(AttributeQualifier) },
            { 0x81, typeof(PagePositionInformation) },
            { 0x82, typeof(ParameterValue) },
            { 0x83, typeof(PresentationControl) },
            { 0x84, typeof(FontResolutionAndMetricTechnology) },
            { 0x85, typeof(FinishingOperation) },
            { 0x86, typeof(TextFidelity) },
            { 0x87, typeof(MediaFidelity) },
            { 0x88, typeof(FinishingFidelity) },
            { 0x8B, typeof(ObjectFontDescriptorData) },
            { 0x8C, typeof(LocaleSelector) },
            { 0x8E, typeof(UP3iFinishingOperation) },
            { 0x91, typeof(ColorManagementResourceDescriptor) },
            { 0x95, typeof(RenderingIntent) },
            { 0x96, typeof(CMRTagFidelity) },
            { 0x97, typeof(DeviceAppearance) },
            { 0x9A, typeof(ImageResolution) },
            { 0x9C, typeof(ObjectContainerPresentationSpaceSize) }
        };
        #endregion

        #region PTX Control Sequences
        public static IReadOnlyDictionary<byte, Type> PTXControlSequences = new Dictionary<byte, Type>()
        {
            { 0x6A, typeof(UCT) },
            { 0x6D, typeof(GLC) },
            { 0x72, typeof(OVS) },
            { 0x73, typeof(OVS) },
            { 0x74, typeof(STC) },
            { 0x75, typeof(STC) },
            { 0x76, typeof(USC) },
            { 0x77, typeof(USC) },
            { 0x78, typeof(TBM) },
            { 0x79, typeof(TBM) },
            { 0x80, typeof(SEC) },
            { 0x81, typeof(SEC) },
            { 0x8B, typeof(GIR) },
            { 0x8C, typeof(GAR) },
            { 0x8D, typeof(GAR) },
            { 0x8E, typeof(GOR) },
            { 0x8F, typeof(GOR) },
            { 0xC0, typeof(SIM) },
            { 0xC1, typeof(SIM) },
            { 0xC2, typeof(SIA) },
            { 0xC3, typeof(SIA) },
            { 0xC4, typeof(SVI) },
            { 0xC5, typeof(SVI) },
            { 0xC6, typeof(AMI) },
            { 0xC7, typeof(AMI) },
            { 0xC8, typeof(RMI) },
            { 0xC9, typeof(RMI) },
            { 0xD0, typeof(SBI) },
            { 0xD1, typeof(SBI) },
            { 0xD2, typeof(AMB) },
            { 0xD3, typeof(AMB) },
            { 0xD4, typeof(RMB) },
            { 0xD5, typeof(RMB) },
            { 0xD8, typeof(BLN) },
            { 0xD9, typeof(BLN) },
            { 0xDA, typeof(TRN) },
            { 0xDB, typeof(TRN) },
            { 0xE4, typeof(DIR) },
            { 0xE5, typeof(DIR) },
            { 0xE6, typeof(DBR) },
            { 0xE7, typeof(DBR) },
            { 0xEE, typeof(RPS) },
            { 0xEF, typeof(RPS) },
            { 0xF0, typeof(SCFL) },
            { 0xF1, typeof(SCFL) },
            { 0xF2, typeof(BSU) },
            { 0xF3, typeof(BSU) },
            { 0xF4, typeof(ESU) },
            { 0xF5, typeof(ESU) },
            { 0xF6, typeof(STO) },
            { 0xF7, typeof(STO) },
            { 0xF8, typeof(PTXControlSequences.NOP) },
            { 0xF9, typeof(PTXControlSequences.NOP) }
        };
        public static byte PTXControlSequenceID<T>(bool chained)
        {
            // Return the byte ID of the PTX type (or NOP if not found). Returns the odd or even function depending on chained
            byte hexCode = PTXControlSequences.FirstOrDefault(f => f.Value == typeof(T) && (f.Key % 2 == 1) == chained).Key;
            if (hexCode == 0) hexCode = PTXControlSequences.First(f => f.Value == typeof(PTXControlSequences.NOP)).Key;

            return hexCode;
        }
        #endregion

        #region Image Self Defining Fields
        public static IReadOnlyDictionary<byte, Type> ImageSelfDefiningFields = new Dictionary<byte, Type>()
        {
            { 0x70, typeof(BeginSegment)},
            { 0x71, typeof(EndSegment)},
            { 0x8C, typeof(BeginTile)},
            { 0x8D, typeof(EndTile)},
            { 0x8E, typeof(BeginTransparencyMask)},
            { 0x8F, typeof(EndTransparencyMask)},
            { 0x91, typeof(BeginImageContent)},
            { 0x93, typeof(EndImageContent)},
            { 0x94, typeof(ImageSize)},
            { 0x95, typeof(ImageEncoding)},
            { 0x96, typeof(IDESize)},
            { 0x97, typeof(ImageLUT_ID)},
            { 0x98, typeof(BandImage)},
            { 0x9B, typeof(IDEStructure)},
            { 0x9F, typeof(ExternalAlgorithmSpecification)},
            { 0xB5, typeof(TilePosition)},
            { 0xB6, typeof(TileSize)},
            { 0xB7, typeof(TileSetColor)},
            { 0xF4, typeof(SetExtendedBilevelImageColor)},
            { 0xF6, typeof(SetBilevelImageColor)},
            { 0xF7, typeof(IOCAFunctionSetIdentification)},
            { 0x92, typeof(ImageData)},
            { 0x9C, typeof(BandImageData)},
            { 0xB8, typeof(IncludeTile)},
            { 0xBB, typeof(TileTOC)},
            { 0xCE, typeof(ImageSubsampling)}
        };
        #endregion

        #region Standard OCA Color Table
        public static IReadOnlyDictionary<byte, Color> StandardOCAColors = new Dictionary<byte, Color>()
        {
            { 0x01, Color.Blue },
            { 0x02, Color.Red },
            { 0x03, Color.Magenta },
            { 0x04, Color.Green },
            { 0x05, Color.Cyan },
            { 0x06, Color.Yellow },
            { 0x07, Color.White },
            { 0x08, Color.Black },
            { 0x09, Color.DarkBlue },
            { 0x0A, Color.Orange },
            { 0x0B, Color.Purple },
            { 0x0C, Color.DarkGreen },
            { 0x0D, Color.DarkCyan },
            { 0x0E, Color.DarkGoldenrod },
            { 0x0F, Color.Gray },
            { 0x10, Color.Brown }
        };
        #endregion

        #region MMC Keywords
        public static IReadOnlyDictionary<byte, string> MMCKeywords = new Dictionary<byte, string>()
        {
            { 0x0E, "Horizontal print adjustment" },
            { 0x90, "Media destination selector — high" },
            { 0x91, "Media destination selector — low" },
            { 0xA0, "Medium information" },
            { 0xA1, "Fixed perforation cut" },
            { 0xA2, "Fixed separation cut" },
            { 0xB4, "Presentation subsystem set-up ID: high-order byte" },
            { 0xB5, "Presentation subsystem set-up ID: low-order byte" },
            { 0xD1, "Offset stack/edge mark change" },
            { 0xE0, "Media source selection format" },
            { 0xE1, "Media source selector" },
            { 0xE8, "Media type local ID: high-order byte" },
            { 0xE9, "Media type local ID: low-order byte" },
            { 0xF1, "Forms flash" },
            { 0xF2, "Medium overlay local identifier" },
            { 0xF3, "Text suppression local identifier" },
            { 0xF4, "Duplex control" },
            { 0xF8, "Print quality control" },
            { 0xF9, "Constant forms control" },
            { 0xFC, "N-up format control" }
        };
        #endregion

        #region OIDs
        public static IReadOnlyDictionary<string, string> OIDs = new Dictionary<string, string>()
        {
            { "06072B120004010105", "IOCA FS10" },
            { "06072B12000401010B", "IOCA FS11" },
            { "06072B12000401010C", "IOCA FS45" },
            { "06072B12000401010D", "EPS" },
            { "06072B12000401010E", "TIFF" },
            { "06072B12000401010F", "COM Set-up" },
            { "06072B120004010110", "Tape Label Set-up" },
            { "06072B120004010111", "DIB, Windows Version" },
            { "06072B120004010112", "DIB, OS/2 PM Version" },
            { "06072B120004010113", "PCX" },
            { "06072B120004010114", "Color Mapping Table (CMT)" },
            { "06072B120004010116", "GIF" },
            { "06072B120004010117", "AFPC JPEG Subset" },
            { "06072B120004010118", "AnaStak Control Record" },
            { "06072B120004010119", "PDF Single-page Object" },
            { "06072B12000401011A", "PDF Resource Object" },
            { "06072B120004010122", "PCL Page Object" },
            { "06072B12000401012D", "IOCA FS42" },
            { "06072B12000401012E", "Resident Color Profile" },
            { "06072B12000401012F", "IOCA FS45 Tile Resource" },
            { "06072B120004010130", "EPS with Transparency" },
            { "06072B120004010131", "PDF with Transparency" },
            { "06072B120004010133", "TrueType/OpenType Font" },
            { "06072B120004010135", "TrueType/OpenType Font Collection" },
            { "06072B120004010136", "Resource Access Table" },
            { "06072B120004010137", "IOCA FS40" },
            { "06072B120004010138", "UP3i Print Data" },
            { "06072B120004010139", "Color Management Resource (CMR)" },
            { "06072B12000401013A", "JPEG2000 (JP2) File Format" },
            { "06072B12000401013C", "TIFF without Transparency" },
            { "06072B12000401013D", "TIFF Multiple Image File" },
            { "06072B12000401013E", "TIFF Multiple Image - without Transparency - File" },
            { "06072B12000401013F", "PDF Multiple Page File" },
            { "06072B120004010140", "PDF Multiple Page - with Transparency - File" },
            { "06072B120004010142", "AFPC TIFF Subset" }
        };
        #endregion
    }
}