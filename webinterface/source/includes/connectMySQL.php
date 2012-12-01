﻿<?php
try
{
	$pdo = new PDO(MYSQL_DSN, MYSQL_USERNAME, MYSQL_PASSWORD);
	$pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
	$pdo->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_OBJ);
}
catch (PDOException $error)
{
	die("Database connection failed: " . $error);
}
?>