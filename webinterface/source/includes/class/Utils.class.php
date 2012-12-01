<?php
class Utils
{
	/**
	 * Creates or modifies an multidimensional array using the path
	 * @param array &$array The array to manipulate
	 * @param array $path An array containing each path part
	 * @param mixed $value The value which should be assigned to the element
	 * @param bool $ignoreIfExisting Set to true to only set the value if the element does not exist
	 */
	public static function createArrayFromPath(&$array, $path, $value, $ignoreIfExisting = false)
	{
		$pathPart = array_shift($path);
		if (empty($path))
		{
			if (!$ignoreIfExisting or !isset($array[$pathPart]))
			{
				$array[$pathPart] = $value;
			}
		}
		else
		{
			if (!isset($array[$pathPart]) or !is_array($array[$pathPart]))
			{
				$array[$pathPart] = array();
			}
			Utils::createArrayFromPath($array[$pathPart], $path, $value, $ignoreIfExisting);
		}
	}
}