create database ImageTagging;

use ImageTagging;

create table Image (
ID		int		not null	primary key	,
Picture	Image	not null,
Description varchar(1000) null
)

create table Tag (
PictureID	int		not null	foreign key references Image(ID),
Tag			varchar(255)	not null
)

/*
select I.ID, I.Picture, I.Description, T.Tag
from Image I inner join Tag T on I.ID = T.PictureID
order by I.ID asc

select * 
from Image;

delete from Tag;
delete from Image;

drop table Tag;
drop table Image;
*/

