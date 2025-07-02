using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestLife.Migrations
{
    /// <inheritdoc />
    public partial class InitialCleanSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Registration",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Registration",
                columns: new[] { "Id", "ConfirmPassword", "ConfirmationCode", "DOB", "DateJoined", "Email", "FullName", "Gender", "IDNO", "InvitedBy", "IsEmailConfirmed", "MyReferralCode", "Password", "PhoneNumber", "ReferralCode", "Role" },
                values: new object[] { 1, "AQAAAAEAACcQAAAAENXx3qN4oRzoCShd7IrgOAIJZ+2LZdUGwHp/BoJTuMUBWxTZcvKzkJ8Ff7CQz4WzSg==", null, new DateTime(1980, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@bestlife.com", "Admin User", "Male", "12345678", null, true, "ADMINREF", "AQAAAAEAACcQAAAAENXx3qN4oRzoCShd7IrgOAIJZ+2LZdUGwHp/BoJTuMUBWxTZcvKzkJ8Ff7CQz4WzSg==", "0700000000", null, "Admin" });
        }
    }
}
