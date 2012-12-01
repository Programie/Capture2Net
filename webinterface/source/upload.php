<?php
require_once "includes/userData.php";
require_once "includes/class/Format.class.php";

// Check if the user is logged in
$userData = getUserData();
if (!$userData)
{
	header("HTTP/1.1 403 Forbidden");
	die("Login error!");
}

// Search info data block
$infoDataStartKey = "\tINFODATA\t";
$fileContent = file_get_contents("php://input");
$infoDataStart = strpos($fileContent, $infoDataStartKey);
if ($infoDataStart === false)
{
	header("HTTP/1.1 400 Bad Request");
	die("Info data block not found!");
}

// Read info data
$infoData = explode("\n", trim(substr($fileContent, $infoDataStart + strlen($infoDataStartKey))));
foreach ($infoData as $index => $line)
{
	unset($infoData[$index]);
	$line = explode("=", $line);
	$infoData[trim($line[0])] = trim($line[1]);
}

// Fix username string (Convert DOMAIN\Username to Username)
$userName = explode("\\", $infoData["userName"], 2);
if (count($userName) == 2)
{
	array_shift($userName);
}
$infoData["userName"] = $userName[0];

// Check if the screenshot type if valid
$config = $userData->configData["screenshots"][strtolower($infoData["screenshotType"])];
if (!$config)
{
	header("HTTP/1.1 400 Bad Request");
	die("Invalid type!");
}

// Check if the file extension is valid
if ($infoData["fileExtension"] != "bmp" and $infoData["fileExtension"] != "gif" and $infoData["fileExtension"] != "png" and $infoData["fileExtension"] != "jpg" and $infoData["fileExtension"] != "tif")
{
	header("HTTP/1.1 400 Bad Request");
	die("Invalid file extension!");
}

// Extract image data
if (function_exists("mb_substr"))
{
	$imageData = mb_substr($fileContent, 0, $infoDataStart, "8bit");
}
else
{
	$imageData = substr($fileContent, 0, $infoDataStart);
}

// Get current time
$time = time();

// Get a valid filename
$fileName = Format::getFileName($config["fileName"], $infoData, $time) . "." . $infoData["fileExtension"];
if (file_exists(UPLOAD_PATH . "/" . $fileName))
{
	header("HTTP/1.1 409 Conflict");
	die("The file " . $fileName . " already exists!");
}

// Save the image data
if (file_put_contents(UPLOAD_PATH . "/" . $fileName, $imageData) === false)
{
	header("HTTP/1.1 500 Internal Server Error");
	die("Unable to write the file!");
}

// Write information to database
$query = $pdo->prepare("INSERT INTO `screenshots` (`userId`, `date`, `type`, `fileName`, `activeWindow`, `userName`, `hostName`) VALUES(:userId, :date, :type, :fileName, :activeWindow, :userName, :hostName)");
$query->execute(array
(
	":userId" => $userData->id,
	":date" => date("Y-m-d h:i:s", $time),
	":type" => $infoData["screenshotType"],
	":fileName" => $fileName,
	":activeWindow" => $infoData["activeWindow"],
	":userName" => $infoData["userName"],
	":hostName" => $infoData["hostName"]
));

// Return the URL of the file
echo UPLOAD_URL . "/" . $fileName;
?>