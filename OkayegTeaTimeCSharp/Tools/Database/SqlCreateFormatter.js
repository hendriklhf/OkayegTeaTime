const file = require("fs");

const sqlFilePath = "../../Resources/SqlCreate.sql";
const lineEnding = "\r\n";
const string = {
    "empty": ""
};

main();

function main() {
    try {
        formatSqlFile();
        console.log("Formatted SqlCreate.sql");
    } catch (err) {
        console.error(err);
    }
}

function formatSqlFile() {
    var fileContent = file.readFileSync(sqlFilePath, "utf-8").split(lineEnding);
    var result = string.empty;
    fileContent.forEach(f => {
        if (!f.match(/^--/) && f.match(/\w+/)) {
            result += `${f}${lineEnding}`;
        }
    });
    file.writeFileSync(sqlFilePath, result);
}
