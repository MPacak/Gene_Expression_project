using System;
using System.Collections.Generic;
using System.IO;

public static class CohortNaming
{
    private static readonly Dictionary<string, string> cohortDictionary = new()
    {
        { "LAML", "Acute Myeloid Leukemia" },
        { "ACC", "Adrenocortical Cancer" },
        { "CHOL", "Bile Duct Cancer" },
        { "BLCA", "Bladder Cancer" },
        { "BRCA", "Breast Cancer" },
        { "CESC", "Cervical Cancer" },
        { "COADREAD", "Colon and Rectal Cancer" },
        { "COAD", "Colon Cancer" },
        { "UCEC", "Endometrioid Cancer" },
        { "ESCA", "Esophageal Cancer" },
        { "FPPP", "Formalin Fixed Paraffin-Embedded Pilot Phase II" },
        { "GBM", "Glioblastoma" },
        { "HNSC", "Head and Neck Cancer" },
        { "KICH", "Kidney Chromophobe" },
        { "KIRC", "Kidney Clear Cell Carcinoma" },
        { "KIRP", "Kidney Papillary Cell Carcinoma" },
        { "DLBC", "Large B-cell Lymphoma" },
        { "LIHC", "Liver Cancer" },
        { "LGG", "Lower Grade Glioma" },
        { "GBMLGG", "Lower Grade Glioma and Glioblastoma" },
        { "LUAD", "Lung Adenocarcinoma" },
        { "LUNG", "Lung Cancer" },
        { "LUSC", "Lung Squamous Cell Carcinoma" },
        { "SKCM", "Melanoma" },
        { "MESO", "Mesothelioma" },
        { "UVM", "Ocular Melanomas" },
        { "OV", "Ovarian Cancer" },
        { "PANCAN", "Pan-Cancer" },
        { "PAAD", "Pancreatic Cancer" },
        { "PCPG", "Pheochromocytoma & Paraganglioma" },
        { "PRAD", "Prostate Cancer" },
        { "READ", "Rectal Cancer" },
        { "SARC", "Sarcoma" },
        { "STAD", "Stomach Cancer" },
        { "TGCT", "Testicular Cancer" },
        { "THYM", "Thymoma" },
        { "THCA", "Thyroid Cancer" },
        { "UCS", "Uterine Carcinosarcoma" }
    };

    public static string GetFullCohortName(string shortName)
    {
        return cohortDictionary.TryGetValue(shortName, out string fullName) ? fullName : shortName;
    }

    public static IEnumerable<string> GetAllShortNames()
    {
        return cohortDictionary.Keys;
    }

}
