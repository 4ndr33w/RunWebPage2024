﻿namespace RunWebPage2024.Models
{
    public class CompanyStatisticModel
    {
        private string _companyName;
        private double _result;
        private double _averageResult;
        private int _rivalsCount;

        public CompanyStatisticModel() { }

        public CompanyStatisticModel(CompanyStatisticModel companyStatisticModel)
        {
            _companyName = companyStatisticModel.CompanyName;
            _result = companyStatisticModel.Result;
            _rivalsCount = companyStatisticModel.RivalsCount;
            _averageResult = companyStatisticModel.AverageResult;
        }

        public string CompanyName { get => _companyName; set => _companyName = value; }
        public double Result { get => _result; set => _result = value; }
        public int RivalsCount { get => _rivalsCount; set => _rivalsCount = value; }
        public double AverageResult { get => _averageResult; set => _averageResult = value; }

    }
}
