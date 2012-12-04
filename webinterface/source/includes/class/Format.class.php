<?php
class Format
{
	public static function getFileName($fileName, $infoData)
	{
		$time = time();
		
		// Replace tokens
		$fileName = str_replace("%Y", date("Y", $time), $fileName);
		$fileName = str_replace("%m", date("m", $time), $fileName);
		$fileName = str_replace("%d", date("d", $time), $fileName);
		$fileName = str_replace("%h", date("H", $time), $fileName);
		$fileName = str_replace("%i", date("i", $time), $fileName);
		$fileName = str_replace("%s", date("s", $time), $fileName);
		$fileName = str_replace("%W", str_replace("%", "%%", $infoData["activeWindow"]), $fileName);
		$fileName = str_replace("%U", str_replace("%", "%%", $infoData["userName"]), $fileName);
		$fileName = str_replace("%H", str_replace("%", "%%", $infoData["hostName"]), $fileName);
		$fileName = str_replace("%%", "%", $fileName);
		
		// Convert the filename into a safe filename (Replace unsafe characters like the slash)
		$allowedCharacters = str_split(FILENAMES_ALLOWEDCHARACTERS);
		$safeFilename = "";
		for ($index = 0; $index < strlen($fileName); $index++)
		{
			$allowed = false;
			$character = substr($fileName, $index, 1);
			foreach ($allowedCharacters as $allowedCharacter)
			{
				if ($character == $allowedCharacter)
				{
					$allowed = true;
					break;
				}
			}
			if ($allowed)
			{
				$safeFilename .= $character;
			}
			else
			{
				if (substr($safeFilename, -1) != "_")
				{
					$safeFilename .= "_";
				}
			}
		}
		return $safeFilename;
	}
}
?>