

(function ($) {
    var cultures = $.global.cultures,
        en = cultures.en,
        standard = en.calendars.standard,
        culture = cultures["vi-VI"] = $.extend(true, {}, en, {
            name: "vi-VI",
            englishName: "Vietnamese",
            nativeName: "Tiếng Việt",
            language: "vi",
            numberFormat: {
                ',': ".",
                '.': ",",
                percent: {
                    pattern: ["-n%", "n%"],
                    ',': ".",
                    '.': ","
                },
                currency: {
                    pattern: ["-n $", "n $"],
                    ',': ".",
                    '.': ",",
                    symbol: "VNĐ"
                }
            },
            calendars: {
                standard: $.extend(true, {}, standard, {
                    '/': "/",
                    firstDay: 1,
                    days: {
                        names: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
                        namesAbbr: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"],
                        namesShort: ["CN", "T2", "T3", "T4", "T5", "T6", "T7"]
                    },
                    months: {
                        names: ["Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12", ""],
                        namesAbbr: ["T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9", "T10", "T11", "T12", ""]
                    },
                    AM: null,
                    PM: null,
                    eras: [{ "name": "n. Chr.", "start": null, "offset": 0}],
                    patterns: {
                        d: "dd/MM/yyyy",
                        D: "dddd, d/ MMMM yyyy",
                        t: "HH:mm",
                        T: "HH:mm:ss",
                        f: "dddd, d/ MMMM yyyy HH:mm",
                        F: "dddd, d/ MMMM yyyy HH:mm:ss",
                        M: "dd MMMM",
                        Y: "MMMM yyyy"
                    }
                })
            }
        }, cultures["vi-VI"]);
    culture.calendar = culture.calendars.standard;
})(jQuery);