<?php
require_once "includes/class/RPCHandler.class.php";

// Get input data and check it
$input = file_get_contents("php://input");
if (!$input)
{
	header("HTTP/1.1 400 Bad Request");
	die("No data received!");
}

// Get json data and check it
$jsonData = json_decode($input);
if (!$jsonData)
{
	header("HTTP/1.1 400 Bad Request");
	die("No JSON data received!");
}

// Initialize RPCHandler
$rpcHandler = new RPCHandler();

// Check if the called method exists
if (!method_exists($rpcHandler, $jsonData->method))
{
	header("HTTP/1.1 400 Bad Request");
	die("Invalid method!");
}

// Check if the user is logged in
if ($json->method != "login" and !$rpcHandler->checkLogin(null))
{
	die(json_encode(array
	(
		"result" => RETURN_LOGINREQUIRED,
		"error" => null,
		"id" => $jsonData->id
	)));
}

// Call the method
$returnData = $rpcHandler->{$jsonData->method}($jsonData->params);

echo json_encode(array
(
	"result" => $returnData,
	"error" => null,
	"id" => $jsonData->id
));
?>