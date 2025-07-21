﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobBoardApi.Migrations
{
    /// <inheritdoc />
    public partial class AddIsApprovedToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Jobs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Jobs");
        }
    }
}
