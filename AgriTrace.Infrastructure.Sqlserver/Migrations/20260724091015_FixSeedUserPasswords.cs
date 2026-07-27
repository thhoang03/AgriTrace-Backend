using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgriTrace.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000005"),
                column: "PasswordHash",
                value: "100000.kvsoLjDUX9yZUnO8qQ25bA==.WfdZNGvGSz5VLRa6KbjtFdMlu+Ac0wFDRY4OLYjZsxw=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000006"),
                column: "PasswordHash",
                value: "100000.GU+TnccSHk98rWkw0cQXEw==.ebiU7auk5qcdnKJlpraFuOV/h+ev7/Q9rUF9SjkQolk=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000007"),
                column: "PasswordHash",
                value: "100000.ZCEfWZ2DeDBafl7sSoMR+w==.vS5N2A5Y2xxC3dQylLJes39s1xHrhN/mNbLz5D1/KVo=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000009"),
                column: "PasswordHash",
                value: "100000.a1msUgi5QIhbZpEXRt1RLw==.pBgsOLFf8fuOYD0kIrZE1QjF3Zw5mNqwItZaWY0Nu+A=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000a"),
                column: "PasswordHash",
                value: "100000.jaBwHVU9d0AVr3qmA2kLWw==.IdJpm8+lGT/Z25/5KZl5eoOK9SQ+keuCylWiWKYMgKY=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000b"),
                column: "PasswordHash",
                value: "100000.44b24E3eLoX/Tn/6Ss3n/w==.7rXza6oxS2cigQoMHeKjYexG9a3ZT5FWdzQY1gApa/Q=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000c"),
                column: "PasswordHash",
                value: "100000.p1jNharWhbkY18w7UEmMNQ==.h5cbCKyGjyeFUhtrKcOOXPT0nNpAcAtrSZJY3GtOV3Y=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000d"),
                column: "PasswordHash",
                value: "100000.po4yuFgTHgXEsj6fk4vYBQ==.0eVPEQ+X+wtF52FX7Y6T6LSlMzE7+eV46LYl6Yglff0=");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000e"),
                column: "PasswordHash",
                value: "100000.jlzfdeBqq5BkzvHyVkVUkw==.R3KvBcnhxzGqNbNFgJG3HPc1ozb7L+8OUUHjIUQAgOw=");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000005"),
                column: "PasswordHash",
                value: "100000.abc123def456ghi789jkl012mno345pqr678stu901vwx234yzA5678==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000006"),
                column: "PasswordHash",
                value: "100000.xyz789abc012def345ghi678jkl901mno234pqr567stu890vwx123yzA4567==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000007"),
                column: "PasswordHash",
                value: "100000.sys0000ops0000sys0000ops0000sys0000ops0000sys0000ops0000==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-000000000009"),
                column: "PasswordHash",
                value: "100000.mgrA1000farm0000mgrA1000farm0000mgrA1000farm0000==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000a"),
                column: "PasswordHash",
                value: "100000.stfB2000bean0000stfB2000bean0000stfB2000bean0000==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000b"),
                column: "PasswordHash",
                value: "100000.mgrD4000link0000mgrD4000link0000mgrD4000link0000==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000c"),
                column: "PasswordHash",
                value: "100000.mgrC3000qual0000mgrC3000qual0000mgrC3000qual0000==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000d"),
                column: "PasswordHash",
                value: "100000.stfE5000mart0000stfE5000mart0000stfE5000mart0000==");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("70000000-0000-0000-0000-00000000000e"),
                column: "PasswordHash",
                value: "100000.stfF6000sys0000stfF6000sys0000stfF6000sys0000==");
        }
    }
}
