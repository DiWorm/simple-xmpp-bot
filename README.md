# simple-xmpp-bot
Simple XMPP Bot on C-Sharp + PHP Plugins.

#For the bot required library agsxmpp-sdk
http://www.ag-software.net/agsxmpp-sdk/

# How it work
bot will check all rows in the chat and will respond to the "Bot: ", "Bot: text", "!cmd", "!cmd parameter".

#Plugins
This bot is working with plugins in PHP.

#Simple example
You: !test (NOT A "BOT: !test")

in php-plugin:
```
<?
header('Content-Type: text/html; charset=utf-8');

$cmd = trim($_POST['cmd']);
$what = trim($_POST['what']);
$perm = trim($_POST['perm']);

if($cmd == "!test)
  { echo "it works! }
?>
```

#Example with checking permisions
```
<?
header('Content-Type: text/html; charset=utf-8');

$cmd = trim($_POST['cmd']);
$what = trim($_POST['what']);
$perm = trim($_POST['perm']);

if($cmd == "!test && $perm == "owner")
  { echo "it works!; }
else
  { echo "You cant do this!"; }
?>
```

#Plugin !google txt
```
<?
header('Content-Type: text/html; charset=utf-8');

$cmd = trim($_POST['cmd']);
$what = trim($_POST['what']);
$perm = trim($_POST['perm']);

if($perm == "owner" || $perm == "admin" || $perm == "member")
	{
		$x = json_decode( file_get_contents( 'http://ajax.googleapis.com/ajax/services/search/web?v=1.0&q=' . urlencode($what)));
		echo $x->responseData->results[0]->unescapedUrl;
	}
else
	echo "You cant do this!";
?>
```

#ALSO
You can do talk bot.

For this you must use $cmd = talks in your PHP

Sample with MySQL
```
if($cmd == "talks")
	{
		if($perm == "owner" || $perm == "admin" || $perm == "member")
		{
		
			$db  = new mysql(); 
			if(!$db->IsConnected())
				exit("db error");
			
			if(!empty($what) && $what != NULL && $what != " " && strlen($what) >= 5)
				{
				$what = htmlspecialchars(trim($what));
				$q = "INSERT INTO  `db`.`tbl` (`id` ,`text`)VALUES (NULL ,  '{$what}');";
				$db->query($q);
				}
			
			$q = "SELECT * FROM `tbl` AS `r1`
					JOIN (SELECT (RAND()*(SELECT MAX(`id`)-MIN(`id`) FROM `tbl`) +
					(SELECT MIN(`id`) FROM `tbl`)) AS `id`) AS `r2`
					WHERE `r1`.`id` >= `r2`.`id` ORDER BY `r1`.`id` ASC LIMIT 1";
			$r = $db->query($q);
			$db->close();
			echo htmlspecialchars_decode(trim($r->text));
		}
		else
			echo "You cant do this!";		
	}
```

If you have big DB with images -  you can do somthing like *talks* - $cmd = "images"
put image in bot and bot answer to you image fromdb

#Support
I will not explain to everyone how it works, the code is very simple, but still if you have any questions:
email: dyworm [at] gmail [dot] com
xmpp-chat: gamecoma@conference.jabber.ru
site: https://gamecoma.ru
