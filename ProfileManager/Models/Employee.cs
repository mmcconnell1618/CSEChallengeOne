using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfileManager.Models
{
    /// <summary>
    /// Model class that hold information about an employee
    /// </summary>
    public class Employee
    {
        // Constructor that initialized empty values
        public Employee()
        {
            this.Id = 0;             // Same as default but explicitly set for visibility
            this.FirstName = "";     // initialize instead of null value
            this.LastName = "";      // initialize instead of null value
            this.Department = "";    // initialize instead of null value
            this.PhotoFileName = ""; // initialize instead of null value
        }

        /// <summary>
        /// Employee ID, auto-generated from database as identity column
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The employee's first name
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(255)]
        public string FirstName { get; set; }

        /// <summary>
        /// The employee's last name
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(255)]
        public string LastName { get; set; }

        //TODO: Consider adding a first-class object for department to prevent typos and allow for name changes
        /// <summary>
        /// Department name for employee
        /// /// </summary>
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(255)]
        public string Department { get; set; }

        /// <summary>
        /// The filename of the photo associated with this employee
        /// </summary>
        [Required(AllowEmptyStrings = true)]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [StringLength(255)]
        public string PhotoFileName { get; set; }

        public int PhotoWidth { get; set; }
        public int PhotoHeight { get; set; }

        /// <summary>
        /// Entity Framework Managed timestamp for concurrency
        /// </summary>
        [Timestamp]
        public byte[] RowVersion { get; set; }

    }
}
