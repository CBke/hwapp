using System.Collections.Generic;
using System.Xml.Serialization;

namespace Data
{

    [XmlRoot(ElementName = "titleInfo")]
    public class TitleInfo
    {
        [XmlElement(ElementName = "title")]
        public string Title { get; set; }
        [XmlAttribute(AttributeName = "lang")]
        public string Lang { get; set; }
    }
    [XmlRoot(ElementName = "namePart")]
    public class NamePart
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "roleTerm")]
    public class RoleTerm
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "authority")]
        public string Authority { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "role")]
    public class Role
    {
        [XmlElement(ElementName = "roleTerm")]
        public RoleTerm RoleTerm { get; set; }
    }
    [XmlRoot(ElementName = "name")]
    public class Name
    {
        [XmlElement(ElementName = "namePart")]
        public List<NamePart> NamePart { get; set; }
        [XmlElement(ElementName = "role")]
        public Role Role { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "affiliation")]
        public List<Affiliation> Affiliation { get; set; }
        [XmlAttribute(AttributeName = "ID")]
        public string ID { get; set; }
    }
    [XmlRoot(ElementName = "originInfo")]
    public class OriginInfo
    {
        [XmlElement(ElementName = "dateIssued")]
        public string DateIssued { get; set; }
        [XmlElement(ElementName = "place")]
        public Place Place { get; set; }
        [XmlElement(ElementName = "publisher")]
        public string Publisher { get; set; }
    }
    [XmlRoot(ElementName = "languageTerm")]
    public class LanguageTerm
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "authority")]
        public string Authority { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "language")]
    public class Language
    {
        [XmlElement(ElementName = "languageTerm")]
        public LanguageTerm LanguageTerm { get; set; }
    }
    [XmlRoot(ElementName = "physicalDescription")]
    public class PhysicalDescription
    {
        [XmlElement(ElementName = "extent")]
        public string Extent { get; set; }
    }
    [XmlRoot(ElementName = "identifier")]
    public class Identifier
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "detail")]
    public class Detail
    {
        [XmlElement(ElementName = "number")]
        public string Number { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }
    [XmlRoot(ElementName = "extent")]
    public class Extent
    {
        [XmlElement(ElementName = "start")]
        public string Start { get; set; }
        [XmlElement(ElementName = "end")]
        public string End { get; set; }
        [XmlAttribute(AttributeName = "unit")]
        public string Unit { get; set; }
    }
    [XmlRoot(ElementName = "part")]
    public class Part
    {
        [XmlElement(ElementName = "detail")]
        public List<Detail> Detail { get; set; }
        [XmlElement(ElementName = "extent")]
        public Extent Extent { get; set; }
        [XmlElement(ElementName = "date")]
        public string Date { get; set; }
    }
    [XmlRoot(ElementName = "relatedItem")]
    public class RelatedItem
    {
        [XmlElement(ElementName = "titleInfo")]
        public TitleInfo TitleInfo { get; set; }
        [XmlElement(ElementName = "part")]
        public Part Part { get; set; }
        [XmlElement(ElementName = "identifier")]
        public Identifier Identifier { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }
    [XmlRoot(ElementName = "pubtype")]
    public class Pubtype
    {
        [XmlAttribute(AttributeName = "src")]
        public string Src { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "affiliation")]
    public class Affiliation
    {
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "extension")]
    public class Extension
    {
        [XmlElement(ElementName = "pubtype")]
        public List<Pubtype> Pubtype { get; set; }
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
    }
    [XmlRoot(ElementName = "recordCreationDate")]
    public class RecordCreationDate
    {
        [XmlAttribute(AttributeName = "encoding")]
        public string Encoding { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "recordChangeDate")]
    public class RecordChangeDate
    {
        [XmlAttribute(AttributeName = "encoding")]
        public string Encoding { get; set; }
        [XmlText]
        public string Text { get; set; }
    }
    [XmlRoot(ElementName = "recordInfo")]
    public class RecordInfo
    {
        [XmlElement(ElementName = "recordContentSource")]
        public string RecordContentSource { get; set; }
        [XmlElement(ElementName = "recordCreationDate")]
        public RecordCreationDate RecordCreationDate { get; set; }
        [XmlElement(ElementName = "recordChangeDate")]
        public RecordChangeDate RecordChangeDate { get; set; }
        [XmlElement(ElementName = "recordIdentifier")]
        public string RecordIdentifier { get; set; }
    }
    [XmlRoot(ElementName = "mods")]
    public class Mods
    {
        [XmlElement(ElementName = "titleInfo")]
        public TitleInfo TitleInfo { get; set; }
        [XmlElement(ElementName = "name")]
        public List<Name> Name { get; set; }
        [XmlElement(ElementName = "originInfo")]
        public OriginInfo OriginInfo { get; set; }
        [XmlElement(ElementName = "language")]
        public Language Language { get; set; }
        [XmlElement(ElementName = "physicalDescription")]
        public PhysicalDescription PhysicalDescription { get; set; }
        [XmlElement(ElementName = "identifier")]
        public List<Identifier> Identifier { get; set; }
        [XmlElement(ElementName = "relatedItem")]
        public List<RelatedItem> RelatedItem { get; set; }
        [XmlElement(ElementName = "extension")]
        public Extension Extension { get; set; }
        [XmlElement(ElementName = "recordInfo")]
        public RecordInfo RecordInfo { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
    }
    [XmlRoot(ElementName = "place")]
    public class Place
    {
        [XmlElement(ElementName = "placeTerm")]
        public string PlaceTerm { get; set; }
    }
    [XmlRoot(ElementName = "modsCollection")]
    public class ModsCollection
    {
        [XmlElement(ElementName = "mods")]
        public List<Mods> Mods { get; set; }
        [XmlAttribute(AttributeName = "xsi", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xsi { get; set; }
        [XmlAttribute(AttributeName = "xlink", Namespace = "http://www.w3.org/2000/xmlns/")]
        public string Xlink { get; set; }
        [XmlAttribute(AttributeName = "xmlns")]
        public string Xmlns { get; set; }
        [XmlAttribute(AttributeName = "schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
        public string SchemaLocation { get; set; }
    }
}