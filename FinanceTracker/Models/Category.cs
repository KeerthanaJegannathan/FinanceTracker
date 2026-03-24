using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace FinanceTracker.Models
{
    public class Category
    {

        public int Id { get; set; } 

        //Name displayed over the charts
        public string Name { get; set; }= string.Empty;

        //Hex color string for UI-coding
        public string Color {  get; set; }=  "#95A5A6";

        //Optional icon name for future Icon support
        public string IconName {  get; set; }= string.Empty;

        //True = built-in default | False = created by user
        public bool IsDefault { get; set; } = true;


        //Define when this category is created
        public string CreatedAt { get; set; } = string.Empty;

        /// <summary>
        /// Restricts which transaction type this category applies to.
        /// 0 = Income only | 1 = Expense only | 2 = Both
        /// </summary>
        public CategoryType Type {  get; set; }

      
    }
    public enum CategoryType
    {
        Income = 0,
        Expense = 1,
        Both = 2
    }
}
