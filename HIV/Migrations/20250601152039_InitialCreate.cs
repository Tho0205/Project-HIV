using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HIV.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    account_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    password_hash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account__46A222CD82E29DFF", x => x.account_id);
                });

            migrationBuilder.CreateTable(
                name: "ARV",
                columns: table => new
                {
                    arv_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ARV__4A3011CB33E11ADC", x => x.arv_id);
                });

            migrationBuilder.CreateTable(
                name: "FacilityInfo",
                columns: table => new
                {
                    facility_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Facility__B2E8EAAEA86B7394", x => x.facility_id);
                });

            migrationBuilder.CreateTable(
                name: "UserTable",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    account_id = table.Column<int>(type: "int", nullable: false),
                    full_name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    birthdate = table.Column<DateOnly>(type: "date", nullable: true),
                    role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__UserTabl__B9BE370F9A1C2C44", x => x.user_id);
                    table.ForeignKey(
                        name: "FK__UserTable__accou__173876EA",
                        column: x => x.account_id,
                        principalTable: "Account",
                        principalColumn: "account_id");
                });

            migrationBuilder.CreateTable(
                name: "Blog",
                columns: table => new
                {
                    blog_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    author_id = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Blog__2975AA28A9AF1AD0", x => x.blog_id);
                    table.ForeignKey(
                        name: "FK__Blog__author_id__1CF15040",
                        column: x => x.author_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "CustomizedARV_Protocol",
                columns: table => new
                {
                    custom_protocol_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    doctor_id = table.Column<int>(type: "int", nullable: true),
                    patient_id = table.Column<int>(type: "int", nullable: true),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customiz__0D356AC88E190C35", x => x.custom_protocol_id);
                    table.ForeignKey(
                        name: "FK__Customize__docto__3B75D760",
                        column: x => x.doctor_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__Customize__patie__3C69FB99",
                        column: x => x.patient_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "DoctorInfo",
                columns: table => new
                {
                    doctor_id = table.Column<int>(type: "int", nullable: false),
                    degree = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    experience_years = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__DoctorIn__F3993564AA01A7F3", x => x.doctor_id);
                    table.ForeignKey(
                        name: "FK__DoctorInf__docto__286302EC",
                        column: x => x.doctor_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "EducationalResources",
                columns: table => new
                {
                    resource_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_by = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Educatio__4985FC7394D73E60", x => x.resource_id);
                    table.ForeignKey(
                        name: "FK__Education__creat__25869641",
                        column: x => x.created_by,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Examination",
                columns: table => new
                {
                    exam_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<int>(type: "int", nullable: true),
                    doctor_id = table.Column<int>(type: "int", nullable: true),
                    result = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    cd4_count = table.Column<int>(type: "int", nullable: true),
                    hiv_load = table.Column<int>(type: "int", nullable: true),
                    exam_date = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Examinat__9C8C7BE9F5579390", x => x.exam_id);
                    table.ForeignKey(
                        name: "FK__Examinati__docto__33D4B598",
                        column: x => x.doctor_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__Examinati__patie__32E0915F",
                        column: x => x.patient_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Reminder",
                columns: table => new
                {
                    reminder_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remind_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Reminder__E27A36289093D052", x => x.reminder_id);
                    table.ForeignKey(
                        name: "FK__Reminder__user_i__37A5467C",
                        column: x => x.user_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    schedule_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    doctor_id = table.Column<int>(type: "int", nullable: true),
                    scheduled_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    room = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, defaultValue: "Available")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__C46A8A6F7368054B", x => x.schedule_id);
                    table.ForeignKey(
                        name: "FK__Schedule__doctor__2B3F6F97",
                        column: x => x.doctor_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    blog_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<int>(type: "int", nullable: true),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Comment__E7957687F7C7DD95", x => x.comment_id);
                    table.ForeignKey(
                        name: "FK__Comment__blog_id__20C1E124",
                        column: x => x.blog_id,
                        principalTable: "Blog",
                        principalColumn: "blog_id");
                    table.ForeignKey(
                        name: "FK__Comment__user_id__21B6055D",
                        column: x => x.user_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "CustomizedARV_Protocol_Detail",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    custom_protocol_id = table.Column<int>(type: "int", nullable: false),
                    arv_id = table.Column<int>(type: "int", nullable: false),
                    dosage = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    usage_instruction = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customiz__3213E83FF6DAC44A", x => x.id);
                    table.ForeignKey(
                        name: "FK__Customize__arv_i__49C3F6B7",
                        column: x => x.arv_id,
                        principalTable: "ARV",
                        principalColumn: "arv_id");
                    table.ForeignKey(
                        name: "FK__Customize__custo__48CFD27E",
                        column: x => x.custom_protocol_id,
                        principalTable: "CustomizedARV_Protocol",
                        principalColumn: "custom_protocol_id");
                });

            migrationBuilder.CreateTable(
                name: "Prescription",
                columns: table => new
                {
                    prescription_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<int>(type: "int", nullable: false),
                    doctor_id = table.Column<int>(type: "int", nullable: false),
                    examination_id = table.Column<int>(type: "int", nullable: false),
                    custom_protocol_id = table.Column<int>(type: "int", nullable: true),
                    exam_date = table.Column<DateOnly>(type: "date", nullable: true),
                    exam_time = table.Column<TimeOnly>(type: "time", nullable: true),
                    issued_at = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Prescrip__3EE444F8F7470B4E", x => x.prescription_id);
                    table.ForeignKey(
                        name: "FK__Prescript__custo__45F365D3",
                        column: x => x.custom_protocol_id,
                        principalTable: "CustomizedARV_Protocol",
                        principalColumn: "custom_protocol_id");
                    table.ForeignKey(
                        name: "FK__Prescript__docto__440B1D61",
                        column: x => x.doctor_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                    table.ForeignKey(
                        name: "FK__Prescript__exami__44FF419A",
                        column: x => x.examination_id,
                        principalTable: "Examination",
                        principalColumn: "exam_id");
                    table.ForeignKey(
                        name: "FK__Prescript__patie__4316F928",
                        column: x => x.patient_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id");
                });

            migrationBuilder.CreateTable(
                name: "Appointment",
                columns: table => new
                {
                    appointment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    patient_id = table.Column<int>(type: "int", nullable: false),
                    schedule_id = table.Column<int>(type: "int", nullable: false),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    is_anonymous = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    appointment_date = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    doctor_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Appointm__A50828FC82CDE625", x => x.appointment_id);
                    table.ForeignKey(
                        name: "FK__Appointme__patie__2F10007B",
                        column: x => x.patient_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__Appointme__sched__300424B4",
                        column: x => x.schedule_id,
                        principalTable: "Schedule",
                        principalColumn: "schedule_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_appointment_doctor",
                        column: x => x.doctor_id,
                        principalTable: "UserTable",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Account__AB6E6164353CD3C1",
                table: "Account",
                column: "email",
                unique: true,
                filter: "[email] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Account__F3DBC572CF8AE374",
                table: "Account",
                column: "username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_doctor_id",
                table: "Appointment",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_patient_id",
                table: "Appointment",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_schedule_id",
                table: "Appointment",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_author_id",
                table: "Blog",
                column: "author_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_blog_id",
                table: "Comment",
                column: "blog_id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_user_id",
                table: "Comment",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizedARV_Protocol_doctor_id",
                table: "CustomizedARV_Protocol",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizedARV_Protocol_patient_id",
                table: "CustomizedARV_Protocol",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizedARV_Protocol_Detail_arv_id",
                table: "CustomizedARV_Protocol_Detail",
                column: "arv_id");

            migrationBuilder.CreateIndex(
                name: "IX_CustomizedARV_Protocol_Detail_custom_protocol_id",
                table: "CustomizedARV_Protocol_Detail",
                column: "custom_protocol_id");

            migrationBuilder.CreateIndex(
                name: "IX_EducationalResources_created_by",
                table: "EducationalResources",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_Examination_doctor_id",
                table: "Examination",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Examination_patient_id",
                table: "Examination",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_custom_protocol_id",
                table: "Prescription",
                column: "custom_protocol_id");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_doctor_id",
                table: "Prescription",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "IX_Prescription_patient_id",
                table: "Prescription",
                column: "patient_id");

            migrationBuilder.CreateIndex(
                name: "UQ__Prescrip__BCD8253117FF2B27",
                table: "Prescription",
                column: "examination_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reminder_user_id",
                table: "Reminder",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_doctor_id",
                table: "Schedule",
                column: "doctor_id");

            migrationBuilder.CreateIndex(
                name: "UQ__UserTabl__46A222CC66B4C8A6",
                table: "UserTable",
                column: "account_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Appointment");

            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropTable(
                name: "CustomizedARV_Protocol_Detail");

            migrationBuilder.DropTable(
                name: "DoctorInfo");

            migrationBuilder.DropTable(
                name: "EducationalResources");

            migrationBuilder.DropTable(
                name: "FacilityInfo");

            migrationBuilder.DropTable(
                name: "Prescription");

            migrationBuilder.DropTable(
                name: "Reminder");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Blog");

            migrationBuilder.DropTable(
                name: "ARV");

            migrationBuilder.DropTable(
                name: "CustomizedARV_Protocol");

            migrationBuilder.DropTable(
                name: "Examination");

            migrationBuilder.DropTable(
                name: "UserTable");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
