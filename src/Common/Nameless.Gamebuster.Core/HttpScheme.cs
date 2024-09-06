using System.ComponentModel;

namespace Nameless.Gamebuster;

public enum HttpScheme {
    [Description]
    None,

    [Description("http")]
    Http,

    [Description("https")]
    Https
}
