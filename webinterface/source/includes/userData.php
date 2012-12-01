<?php
require_once "includes/config.inc.php";
require_once "includes/connectMySQL.php";
require_once "includes/class/Utils.class.php";

function getUserData()
{
	global $pdo;

	$query = $pdo->prepare("SELECT * FROM `users` WHERE `username` = :username AND `password` = :password");
	$query->execute(array
	(
		":username" => $_SERVER["PHP_AUTH_USER"],
		":password" => md5($_SERVER["PHP_AUTH_PW"])
	));

	$row = $query->fetch();
	if (!$row)
	{
		return null;
	}
	$userConfig = json_decode($row->configData);
	Utils::createArrayFromPath($userConfig, array("screenshots", "screen", "imageFormat"), "jpg", true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "screen", "fileName"), "%Y-%m-%d_%h-%i-%s", true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "screen", "shortcut", "control"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "screen", "shortcut", "alt"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "screen", "shortcut", "shift"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "screen", "shortcut", "windows"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "screen", "shortcut", "key"), 44, true);// 44 = PrintScreen
	Utils::createArrayFromPath($userConfig, array("screenshots", "selection", "imageFormat"), "jpg", true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "selection", "fileName"), "%Y-%m-%d_%h-%i-%s", true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "selection", "shortcut", "control"), true, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "selection", "shortcut", "alt"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "selection", "shortcut", "shift"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "selection", "shortcut", "windows"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "selection", "shortcut", "key"), 44, true);// 44 = PrintScreen
	Utils::createArrayFromPath($userConfig, array("screenshots", "window", "imageFormat"), "jpg", true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "window", "fileName"), "%W_%Y-%m-%d_%h-%i-%s", true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "window", "shortcut", "control"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "window", "shortcut", "alt"), true, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "window", "shortcut", "shift"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "window", "shortcut", "windows"), false, true);
	Utils::createArrayFromPath($userConfig, array("screenshots", "window", "shortcut", "key"), 44, true);// 44 = PrintScreen
	$row->configData = $userConfig;
	return $row;
}