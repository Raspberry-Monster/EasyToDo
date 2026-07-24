using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyToDo.Migrations
{
    /// <inheritdoc />
    public partial class AddNotifyAtColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "NotifyAt",
                table: "TaskItems",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotifyAt",
                table: "TaskItems");
        }
    }
}
