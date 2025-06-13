using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CyberHub.Migrations
{
    /// <inheritdoc />
    public partial class NewCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Icon", "Name" },
                values: new object[,]
                {
                    { 4, "Firewalls, IDS/IPS, VPNs, and secure protocols", null, "Network Security" },
                    { 5, "Securing software, code review, OWASP topics", null, "Application Security" },
                    { 6, "Threats and protections in AWS, Azure, GCP...", null, "Cloud Security" },
                    { 7, "Security concerns on Android and iOS", null, "Mobile Security" },
                    { 8, "Capture the Flag events, walkthroughs, and practice", null, "CTFs & Challenges" },
                    { 9, "Courses, books, blogs, and guides", null, "Learning Resources" },
                    { 10, "White-hat hacking techniques and certifications", null, "Ethical Hacking" },
                    { 11, "Advice on certs like CISSP, CEH, OSCP...", null, "Careers & Certifications" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 11);
        }
    }
}
