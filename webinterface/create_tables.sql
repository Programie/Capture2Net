CREATE TABLE IF NOT EXISTS `screenshots`
(
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`userId` int(11) NOT NULL,
	`date` datetime NOT NULL,
	`type` varchar(10) NOT NULL,
	`fileName` varchar(200) NOT NULL,
	`activeWindow` text NOT NULL,
	`userName` varchar(256) NOT NULL,
	`hostName` varchar(15) NOT NULL,
	PRIMARY KEY (`id`)
);

CREATE TABLE IF NOT EXISTS `users`
(
	`id` int(11) NOT NULL AUTO_INCREMENT,
	`userName` varchar(50) NOT NULL,
	`password` varchar(32) NOT NULL,
	`sessionId` varchar(32) NOT NULL,
	`configData` text NOT NULL,
	PRIMARY KEY (`id`),
	UNIQUE KEY `userName` (`userName`)
);