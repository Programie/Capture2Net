<?php
/**************************************
 *                                    *
 *      DO NOT MODIFY THIS FILE!      *
 * CREATE A COPY NAMED config.inc.php *
 *                                    *
 **************************************/

// MySQL
define("MYSQL_DSN", "mysql:host=localhost;dbname=capture2net");// Hostname and database
define("MYSQL_USERNAME", "root");// Username
define("MYSQL_PASSWORD", "");// Password

// Upload target
define("UPLOAD_PATH", "/var/www/capture2net_uploads");// Path where the screenshots should be saved
define("UPLOAD_URL", "http://example.com/capture2net_uploads");// URL to the directory specified in UPLOAD_PATH

// Filenames
define("FILENAMES_ALLOWEDCHARACTERS", "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-");// Character map with valid characters in filenames (Any other character will be replaced with the underscore)
?>