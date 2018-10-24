create table Image (
ID		int		not null	primary key,
Picture	Image	not null
)

create table Tag (
PictureID	int		not null	foreign key references Image(ID),
Tag			varchar(255)	not null
)

select I.ID, I.Picture, T.Tag
from Image I inner join Tag T on I.ID = T.PictureID
order by I.ID asc

delete from Tag;
delete from Image;