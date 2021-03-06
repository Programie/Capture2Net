<?php
require_once "includes/config.inc.php";

define("RETURN_LOGINREQUIRED", "login_required");
define("RETURN_LOGINFAILED", "login_failed");
define("RETURN_OK", "ok");

class RPCHandler
{
	private $pdo;
	private $userId;
	private $isAdmin;
	
	public function __construct()
	{
		try
		{
			$this->pdo = new PDO(MYSQL_DSN, MYSQL_USERNAME, MYSQL_PASSWORD);
			$this->pdo->setAttribute(PDO::ATTR_ERRMODE, PDO::ERRMODE_EXCEPTION);
			$this->pdo->setAttribute(PDO::ATTR_DEFAULT_FETCH_MODE, PDO::FETCH_OBJ);
		}
		catch (PDOException $error)
		{
			header("HTTP/1.1 500 Internal Server Error");
			die("Database connection failed: " . $error);
		}
	}
	
	public function checkLogin($params)
	{
		$query = $this->pdo->prepare("SELECT `id`, `isAdmin` FROM `users` WHERE `sessionId` = :sessionId");
		$query->execute(array
		(
			":sessionId" => $_COOKIE["sessionId"]
		));
		$row = $query->fetch();
		if (!$row)
		{
			return false;
		}
		$this->userId = $row->id;
		$this->isAdmin = $row->isAdmin;
		return true;
	}
	
	public function getScreenshots($params)
	{
		$query = $this->pdo->prepare("SELECT * FROM `screenshots` WHERE `userId` = :userId");
		$query->execute(array
		(
			":userId" => $this->userId
		));
		return array
		(
			"url" => UPLOAD_URL,
			"screenshots" => $query->fetchAll()
		);
	}
	
	public function isAdmin()
	{
		return $this->isAdmin;
	}
	
	public function loadConfig()
	{
		$query = $this->pdo->prepare("SELECT `configData` FROM `users` WHERE `id` = :id");
		$query->execute(array
		(
			":id" => $this->id
		));
		$row = $query->fetch();
		if (!$row->configData)
		{
			return array("_jsonBugFix" => true);
		}
		return json_decode($row->configData);
	}
	
	public function login($params)
	{
		$query = $this->pdo->prepare("SELECT `id` FROM `users` WHERE `userName` = :userName AND `password`= :password");
		$query->execute(array
		(
			":userName" => $params->userName,
			":password" => md5($params->password)
		));
		$row = $query->fetch();
		if (!$row)
		{
			return RETURN_LOGINFAILED;
		}
		$sessionId = md5($row->id . "_" . time());
		$query = $this->pdo->prepare("UPDATE `users` SET `sessionId` = :sessionId WHERE `id` = :id");
		$query->execute(array
		(
			":sessionId" => $sessionId,
			":id" => $row->id
		));
		setcookie("sessionId", $sessionId);
		return RETURN_OK;
	}
	
	public function logout($params)
	{
		setcookie("sessionId", "", time() - 3600);
		$query = $this->pdo->prepare("UPDATE `users` SET `sessionId` = '' WHERE `id` = :id");
		$query->execute(array
		(
			":id" => $this->userId
		));
	}
}
?>