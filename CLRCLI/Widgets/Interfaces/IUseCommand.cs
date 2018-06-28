using System.Xml.Serialization;

namespace CLRCLI.Widgets
{
    internal interface IUseCommand
    {
        [XmlAttribute]
        string Command { get; set; }
    }
}