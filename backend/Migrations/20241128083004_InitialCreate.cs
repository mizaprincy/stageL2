using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Poste_travail",
                columns: table => new
                {
                    id_poste = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    design = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    salaire_horaire = table.Column<decimal>(type: "decimal(60,2)", nullable: false),
                    heures_travaillees = table.Column<int>(type: "int", nullable: false),
                    statut = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Poste_travail", x => x.id_poste);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Employe",
                columns: table => new
                {
                    id_employe = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    nom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    prenom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tel = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    date_embauche = table.Column<DateOnly>(type: "date", nullable: false),
                    date_depart = table.Column<DateOnly>(type: "date", nullable: true),
                    id_poste = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employe", x => x.id_employe);
                    table.ForeignKey(
                        name: "FK_Employe_Poste_travail_id_poste",
                        column: x => x.id_poste,
                        principalTable: "Poste_travail",
                        principalColumn: "id_poste",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Avance",
                columns: table => new
                {
                    AvanceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    id_employe = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MontantTotal = table.Column<decimal>(type: "decimal(60,2)", nullable: false),
                    MontantRestant = table.Column<decimal>(type: "decimal(60,2)", nullable: false),
                    TranchesTotales = table.Column<int>(type: "int", nullable: false),
                    TranchesRestantes = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Statut = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateDebut = table.Column<DateOnly>(type: "date", nullable: false),
                    DateProchaineTranche = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avance", x => x.AvanceId);
                    table.ForeignKey(
                        name: "FK_Avance_Employe",
                        column: x => x.id_employe,
                        principalTable: "Employe",
                        principalColumn: "id_employe",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Salaire",
                columns: table => new
                {
                    id_salaire = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    mois = table.Column<int>(type: "int", nullable: false),
                    annee = table.Column<int>(type: "int", nullable: false),
                    id_employe = table.Column<string>(type: "varchar(100)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    montant = table.Column<decimal>(type: "decimal(60,2)", nullable: false),
                    date_paiement = table.Column<DateOnly>(type: "date", nullable: false),
                    avance = table.Column<decimal>(type: "decimal(60,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Salaire", x => new { x.id_salaire, x.mois, x.annee });
                    table.ForeignKey(
                        name: "FK_Salaire_Employe_id_employe",
                        column: x => x.id_employe,
                        principalTable: "Employe",
                        principalColumn: "id_employe",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prenom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Role = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeId = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_User_Employe",
                        column: x => x.EmployeId,
                        principalTable: "Employe",
                        principalColumn: "id_employe",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DemandeAvance",
                columns: table => new
                {
                    DemandeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmployeId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeUserId = table.Column<int>(type: "int", nullable: false),
                    Montant = table.Column<decimal>(type: "decimal(60,2)", nullable: false),
                    NombreTranches = table.Column<int>(type: "int", nullable: false),
                    DateDemande = table.Column<DateOnly>(type: "date", nullable: false),
                    Statut = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeAvance = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Message = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DemandeAvance", x => new { x.EmployeId, x.DemandeId });
                    table.ForeignKey(
                        name: "FK_DemandeAvance_Employe",
                        column: x => x.EmployeUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Inbox",
                columns: table => new
                {
                    InboxId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AdminId = table.Column<int>(type: "int", nullable: true),
                    EmployeId = table.Column<string>(type: "varchar(100)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EmployeUserId = table.Column<int>(type: "int", nullable: true),
                    DemandeId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateNotification = table.Column<DateOnly>(type: "date", nullable: false),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inbox", x => x.InboxId);
                    table.ForeignKey(
                        name: "FK_Inbox_DemandeAvance_EmployeId_DemandeId",
                        columns: x => new { x.EmployeId, x.DemandeId },
                        principalTable: "DemandeAvance",
                        principalColumns: new[] { "EmployeId", "DemandeId" },
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inbox_User_AdminId",
                        column: x => x.AdminId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Inbox_User_EmployeUserId",
                        column: x => x.EmployeUserId,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Avance_id_employe",
                table: "Avance",
                column: "id_employe");

            migrationBuilder.CreateIndex(
                name: "IX_DemandeAvance_EmployeUserId",
                table: "DemandeAvance",
                column: "EmployeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Employe_id_poste",
                table: "Employe",
                column: "id_poste");

            migrationBuilder.CreateIndex(
                name: "IX_Inbox_AdminId",
                table: "Inbox",
                column: "AdminId");

            migrationBuilder.CreateIndex(
                name: "IX_Inbox_EmployeId_DemandeId",
                table: "Inbox",
                columns: new[] { "EmployeId", "DemandeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Inbox_EmployeUserId",
                table: "Inbox",
                column: "EmployeUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Salaire_id_employe",
                table: "Salaire",
                column: "id_employe");

            migrationBuilder.CreateIndex(
                name: "IX_User_EmployeId",
                table: "User",
                column: "EmployeId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avance");

            migrationBuilder.DropTable(
                name: "Inbox");

            migrationBuilder.DropTable(
                name: "Salaire");

            migrationBuilder.DropTable(
                name: "DemandeAvance");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Employe");

            migrationBuilder.DropTable(
                name: "Poste_travail");
        }
    }
}
