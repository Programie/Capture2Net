<?php
require_once "includes/userData.php";

header("Content-type: text/plain");

$userData = getUserData();
if ($userData)
{
	echo json_encode($userData->configData);
}
else
{
	header("HTTP/1.1 403 Forbidden");
	echo "Login error!";
}
?>