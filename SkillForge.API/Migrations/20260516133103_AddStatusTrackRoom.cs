using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SkillForge.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusTrackRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Room",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "CertificationTrack",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VerificationStatus",
                table: "Certificates");

            migrationBuilder.AddColumn<int>(
                name: "RoomId",
                table: "Sessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EnrollmentStatusId",
                table: "Enrollments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TrackId",
                table: "Courses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VerificationStatusId",
                table: "Certificates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EnrollmentStatuses",
                columns: table => new
                {
                    EnrollmentStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EnrollmentStatuses", x => x.EnrollmentStatusId);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                });

            migrationBuilder.CreateTable(
                name: "Tracks",
                columns: table => new
                {
                    TrackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrackName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tracks", x => x.TrackId);
                });

            migrationBuilder.CreateTable(
                name: "VerificationStatuses",
                columns: table => new
                {
                    VerificationStatusId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationStatuses", x => x.VerificationStatusId);
                });

            migrationBuilder.InsertData(
                table: "EnrollmentStatuses",
                columns: new[] { "EnrollmentStatusId", "StatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Approved" },
                    { 3, "Rejected" },
                    { 4, "Completed" },
                    { 5, "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomId", "Capacity", "Location", "RoomName" },
                values: new object[,]
                {
                    { 1, 25, "Building A", "Lab 101" },
                    { 2, 25, "Building A", "Lab 102" },
                    { 3, 30, "Building B", "Room 201" }
                });

            migrationBuilder.InsertData(
                table: "Tracks",
                columns: new[] { "TrackId", "Description", "TrackName" },
                values: new object[,]
                {
                    { 1, "Programming and software development courses", "Programming" },
                    { 2, "Database design and SQL courses", "Database" },
                    { 3, "Web application development courses", "Web Development" }
                });

            migrationBuilder.InsertData(
                table: "VerificationStatuses",
                columns: new[] { "VerificationStatusId", "StatusName" },
                values: new object[,]
                {
                    { 1, "Pending" },
                    { 2, "Valid" },
                    { 3, "Revoked" },
                    { 4, "Expired" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_RoomId",
                table: "Sessions",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_EnrollmentStatusId",
                table: "Enrollments",
                column: "EnrollmentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_TrackId",
                table: "Courses",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_VerificationStatusId",
                table: "Certificates",
                column: "VerificationStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_Certificates_VerificationStatuses_VerificationStatusId",
                table: "Certificates",
                column: "VerificationStatusId",
                principalTable: "VerificationStatuses",
                principalColumn: "VerificationStatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Tracks_TrackId",
                table: "Courses",
                column: "TrackId",
                principalTable: "Tracks",
                principalColumn: "TrackId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_EnrollmentStatuses_EnrollmentStatusId",
                table: "Enrollments",
                column: "EnrollmentStatusId",
                principalTable: "EnrollmentStatuses",
                principalColumn: "EnrollmentStatusId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Rooms_RoomId",
                table: "Sessions",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certificates_VerificationStatuses_VerificationStatusId",
                table: "Certificates");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Tracks_TrackId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_EnrollmentStatuses_EnrollmentStatusId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Rooms_RoomId",
                table: "Sessions");

            migrationBuilder.DropTable(
                name: "EnrollmentStatuses");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Tracks");

            migrationBuilder.DropTable(
                name: "VerificationStatuses");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_RoomId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_EnrollmentStatusId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Courses_TrackId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Certificates_VerificationStatusId",
                table: "Certificates");

            migrationBuilder.DropColumn(
                name: "RoomId",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "EnrollmentStatusId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "TrackId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "VerificationStatusId",
                table: "Certificates");

            migrationBuilder.AddColumn<string>(
                name: "Room",
                table: "Sessions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CertificationTrack",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VerificationStatus",
                table: "Certificates",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
